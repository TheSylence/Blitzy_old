// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Blitzy.Messages;
using Blitzy.Model.Shell;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.Model
{
	internal enum CatalogProgressStep
	{
		None,
		Parsing,
		Saving
	}

	internal class CatalogBuilder : BaseObject
	{
		#region Constructor

		public CatalogBuilder( Settings settings )
		{
			CanProcess = ToDispose( new AutoResetEvent( false ) );
			Settings = settings;
			_ItemsToProcess = int.MaxValue;

			IsRunning = true;
			ThreadObject = new Thread( RunThreaded );
			ThreadObject.Priority = ThreadPriority.Lowest;
			ThreadObject.Name = "CatalogThread";
			ThreadObject.IsBackground = true;
			ThreadObject.Start();
		}

		#endregion Constructor

		#region Disposable

		protected override void Dispose( bool managed )
		{
			if( managed )
			{
				IsRunning = false;
				CanProcess.Set();
				ThreadObject.Join();
			}

			base.Dispose( managed );
		}

		#endregion Disposable

		#region Methods

		public void Build()
		{
			lock( LockObject )
			{
				FilesToProcess.Clear();
				FilesToProcess.AddRange( Settings.Folders.SelectMany( f => f.GetFiles() ) );
			}

			if( RuntimeConfig.Tests )
			{
				ProcessFiles();
			}
			else
			{
				CanProcess.Set();
			}
		}

		public void Stop()
		{
		}

		internal void ProcessFiles()
		{
			ShouldStop = false;
			string[] files;
			lock( LockObject )
			{
				files = FilesToProcess.Distinct().ToArray();
			}

			if( files.Length > 0 )
			{
				List<FileEntry> entries = new List<FileEntry>( files.Length );

				IsBuilding = true;
				DispatcherHelper.CheckBeginInvokeOnUI( () => Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.BuildStarted ) ) );

				ItemsProcessed = 0;
				ItemsSaved = 0;
				ProgressStep = CatalogProgressStep.Parsing;
				ItemsToProcess = files.Length;
				string tempPath = Path.GetTempPath();

				foreach( string path in files )
				{
					if( ShouldStop )
					{
						break;
					}

					string filePath = path;
					string ext = Path.GetExtension( filePath );
					string icon = filePath;
					string arguments = string.Empty;
					string fileName = Path.GetFileNameWithoutExtension( filePath );

					if( ext.Equals( ".lnk" ) )
					{
						string tmpFile;
						if( Settings.GetValue<bool>( SystemSetting.BackupShortcuts ) )
						{
							tmpFile = Path.Combine( tempPath, string.Format( "blitzy_{0}", Path.GetFileName( filePath ) ) );
							File.Copy( filePath, tmpFile, true );
						}
						else
						{
							tmpFile = filePath;
						}

						try
						{
							using( ShellShortcut link = new ShellShortcut( tmpFile ) )
							{
								string targetPath = Environment.ExpandEnvironmentVariables( link.Path ).ToLowerInvariant();
								if( string.IsNullOrWhiteSpace( targetPath ) )
								{
									Debug.WriteLine( string.Format( CultureInfo.InvariantCulture, "Failed to get target of {0}", filePath ) );
									continue;
								}

								arguments = link.Arguments;
								filePath = targetPath;
								ext = Path.GetExtension( targetPath );
								icon = Environment.ExpandEnvironmentVariables( link.IconPath );

								if( icon.StartsWith( ",", true, CultureInfo.CurrentUICulture ) )
								{
									icon = Environment.ExpandEnvironmentVariables( targetPath + icon );
								}
							}
						}
						catch( Exception ex )
						{
							LogError( "Exception while resolving shortcut: {0}", ex );
							continue;
						}
						finally
						{
							if( Settings.GetValue<bool>( SystemSetting.BackupShortcuts ) )
							{
								File.Delete( tmpFile );
							}
						}
					}

					if( !string.IsNullOrWhiteSpace( ext ) )
					{
						ext = ext.Substring( 1 );
					}

					// Make sure that the shortcut was resolved correctly
					if( ext.Equals( ".lnk" ) || ext.Equals( "lnk" ) )
					{
						LogWarning( "Shortcut was not resolved: {0}", path );
						Debug.Assert( false );
						continue;
					}

					entries.Add( new FileEntry( filePath, fileName, icon, ext, arguments ) );

					ItemsProcessed++;
					DispatcherHelper.CheckBeginInvokeOnUI( () => Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) ) );
				}

				ProgressStep = CatalogProgressStep.Saving;
				IEnumerable<FileEntry> list = entries.Distinct();

				SQLiteTransaction transaction = Settings.Connection.BeginTransaction( System.Data.IsolationLevel.ReadCommitted );
				try
				{
					using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
					{
						cmd.Transaction = transaction;
						cmd.CommandText = "DELETE FROM files";
						cmd.ExecuteNonQuery();
					}

					const int maxBatchSize = 500; // SQLite limit: SQLITE_MAX_COMPOUND_SELECT
					const int maxParameters = 999; // SQLite limit: SQLITE_MAX_VARIABLE_NUMBER
					const int columns = FileEntry.ParameterCount;
					int batchSize = maxBatchSize;
					int objectCount = list.Count();

					int count = 0;
					int runs = 0;
					if( objectCount * columns <= maxParameters )
					{
						runs = (int)Math.Ceiling( objectCount / (double)batchSize );
					}
					else
					{
						runs = (int)Math.Ceiling( objectCount * columns / (double)maxParameters );
						batchSize = (int)Math.Floor( objectCount / (double)runs );

						if( runs * batchSize < objectCount )
						{
							++runs;
						}
					}

					while( count < runs && !ShouldStop )
					{
						using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
						{
							cmd.Transaction = transaction;
							FileEntry.CreateBatchStatement( cmd, list.Take( batchSize ) );
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}

						list = list.Skip( batchSize );
						++count;
						ItemsSaved += batchSize;
						DispatcherHelper.CheckBeginInvokeOnUI( () => Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) ) );
					}

					if( !ShouldStop )
					{
						transaction.Commit();
					}
				}
				catch( Exception ex )
				{
					LogError( "Failed updating the catalog: {0}", ex );
					transaction.Rollback();
				}

				ProgressStep = CatalogProgressStep.None;
				DispatcherHelper.CheckBeginInvokeOnUI( () => Messenger.Default.Send<CatalogStatusMessage>( new CatalogStatusMessage( CatalogStatus.BuildFinished ) ) );
				IsBuilding = false;
			}
		}

		private void RunThreaded()
		{
			while( IsRunning )
			{
				CanProcess.WaitOne();

				// Only parse files if the thread (and therefor the application)
				// is still running
				if( IsRunning )
				{
					ProcessFiles();
				}
			}
		}

		#endregion Methods

		#region Properties

		private bool _IsBuilding;
		private int _ItemsProcessed;
		private int _ItemsSaved;
		private int _ItemsToProcess;
		private CatalogProgressStep _ProgressStep;

		public bool IsBuilding
		{
			get
			{
				return _IsBuilding;
			}

			set
			{
				if( _IsBuilding == value )
				{
					return;
				}

				RaisePropertyChanging( () => IsBuilding );
				_IsBuilding = value;
				RaisePropertyChanged( () => IsBuilding );
			}
		}

		public int ItemsProcessed
		{
			get
			{
				return _ItemsProcessed;
			}

			set
			{
				if( _ItemsProcessed == value )
				{
					return;
				}

				RaisePropertyChanging( () => ItemsProcessed );
				_ItemsProcessed = value;
				RaisePropertyChanged( () => ItemsProcessed );
			}
		}

		public int ItemsSaved
		{
			get
			{
				return _ItemsSaved;
			}

			set
			{
				if( _ItemsSaved == value )
				{
					return;
				}

				RaisePropertyChanging( () => ItemsSaved );
				_ItemsSaved = value;
				RaisePropertyChanged( () => ItemsSaved );
			}
		}

		public int ItemsToProcess
		{
			get
			{
				return _ItemsToProcess;
			}

			set
			{
				if( _ItemsToProcess == value )
				{
					return;
				}

				RaisePropertyChanging( () => ItemsToProcess );
				_ItemsToProcess = value;
				RaisePropertyChanged( () => ItemsToProcess );
			}
		}

		public CatalogProgressStep ProgressStep
		{
			get
			{
				return _ProgressStep;
			}

			set
			{
				if( _ProgressStep == value )
				{
					return;
				}

				RaisePropertyChanging( () => ProgressStep );
				_ProgressStep = value;
				RaisePropertyChanged( () => ProgressStep );
			}
		}

		#endregion Properties

		#region Attributes

		private readonly Settings Settings;
		private AutoResetEvent CanProcess;
		private List<string> FilesToProcess = new List<string>( 16384 );
		private bool IsRunning;
		private object LockObject = new object();
		private volatile bool ShouldStop = false;
		private Thread ThreadObject;

		#endregion Attributes
	}
}
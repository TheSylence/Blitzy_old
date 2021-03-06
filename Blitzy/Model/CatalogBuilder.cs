﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Blitzy.Messages;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.Model
{
	internal enum CatalogProgressStep
	{
		None,
		Scanning,
		Parsing,
		Saving
	}

	internal class CatalogBuilder : BaseObject
	{
		public CatalogBuilder( DbConnectionFactory factory, Settings settings, IMessenger messenger = null )
		{
			Factory = factory;
			MessengerInstance = messenger ?? Messenger.Default;
			CanProcess = ToDispose( new AutoResetEvent( false ) );
			Settings = settings;
			_ItemsToProcess = int.MaxValue;

			IsRunning = true;
			ThreadObject = new Thread( RunThreaded );
			ThreadObject.SetApartmentState( ApartmentState.STA );
			ThreadObject.Priority = ThreadPriority.Lowest;
			ThreadObject.Name = "CatalogThread";
			ThreadObject.IsBackground = true;
			ThreadObject.Start();

			Trace = new StackTrace( true );
		}

		public void Build()
		{
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
			ShouldStop = true;
		}

		internal void ProcessFiles()
		{
			IsBuilding = true;
			DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CatalogStatusMessage( CatalogStatus.BuildStarted ) ) );

			ItemsScanned = 0;
			ProgressStep = CatalogProgressStep.Scanning;
			FilesToProcess.Clear();
			ItemsToProcess = Settings.Folders.Count;
			foreach( Folder folder in Settings.Folders )
			{
				FilesToProcess.AddRange( folder.GetFiles() );

				++ItemsScanned;
				DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) ) );
			}

			ShouldStop = false;
			string[] files = FilesToProcess.Distinct().ToArray();

			if( files.Length > 0 )
			{
				List<FileEntry> entries = new List<FileEntry>( files.Length );

				ItemsProcessed = 0;
				ItemsSaved = 0;
				ProgressStep = CatalogProgressStep.Parsing;
				ItemsToProcess = files.Length;

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

					//if( fileName.Contains( "Steam" ) )
					//{
					//	Debugger.Break();
					//}

					if( ext != null && ext.Equals( ".lnk" ) )
					{
						string tmpFile;
						if( Settings.GetValue<bool>( SystemSetting.BackupShortcuts ) )
						{
							tmpFile = IOUtils.GetTempFileName( "lnk" );
							File.Copy( filePath, tmpFile, true );
						}
						else
						{
							tmpFile = filePath;
						}

						try
						{
							ShellShortcut link = new ShellShortcut( tmpFile );
							string targetPath = Environment.ExpandEnvironmentVariables( link.Path ).ToLowerInvariant();
							if( string.IsNullOrWhiteSpace( targetPath ) )
							{
								LogDebug( "Failed to get target of {0}", filePath );
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
						catch( Exception ex )
						{
							LogError( "Exception while resolving shortcut {1}: {0}", ex, filePath );
							continue;
						}
						finally
						{
							if( Settings.GetValue<bool>( SystemSetting.BackupShortcuts ) )
							{
								try
								{
									File.Delete( tmpFile );
								}
								catch( Exception )
								{
									LogWarning( "Failed to delete temporary backup of shortcut" );
								}
							}
						}
					}

					if( !string.IsNullOrWhiteSpace( ext ) )
					{
						ext = ext.Substring( 1 );
					}

					// Make sure that the shortcut was resolved correctly
					if( ext != null && ( ext.Equals( ".lnk" ) || ext.Equals( "lnk" ) ) )
					{
						LogWarning( "Shortcut was not resolved: {0}", path );
						Debug.Assert( false );
						continue;
					}

					entries.Add( new FileEntry( filePath, fileName, icon, ext, arguments ) );

					ItemsProcessed++;
					DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) ) );
				}

				ProgressStep = CatalogProgressStep.Saving;
				SaveEntries( entries.Distinct() );

				ProgressStep = CatalogProgressStep.None;
			}

			IsBuilding = false;
			DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CatalogStatusMessage( CatalogStatus.BuildFinished ) ) );
		}

		protected override void Dispose( bool managed )
		{
			if( managed )
			{
				ShouldStop = true;
				IsRunning = false;
				CanProcess.Set();
				ThreadObject.Join();
			}

			base.Dispose( managed );
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

		private void SaveEntries( IEnumerable<FileEntry> list )
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				DbTransaction transaction = connection.BeginTransaction( IsolationLevel.ReadCommitted );
				try
				{
					using( DbCommand cmd = connection.CreateCommand() )
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
					int runs;
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
						using( DbCommand cmd = connection.CreateCommand() )
						{
							cmd.Transaction = transaction;
							FileEntry.CreateBatchStatement( cmd, list.Take( batchSize ) );
							cmd.Prepare();
							cmd.ExecuteNonQuery();
						}

						list = list.Skip( batchSize );
						++count;
						ItemsSaved += batchSize;
						DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) ) );
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
			}
		}

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

				_ItemsSaved = value;
				RaisePropertyChanged( () => ItemsSaved );
			}
		}

		public int ItemsScanned
		{
			get
			{
				return _ItemsScanned;
			}

			set
			{
				if( _ItemsScanned == value )
				{
					return;
				}

				_ItemsScanned = value;
				RaisePropertyChanged( () => ItemsScanned );
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

				_ProgressStep = value;
				RaisePropertyChanged( () => ProgressStep );
			}
		}

		private readonly AutoResetEvent CanProcess;
		private readonly List<string> FilesToProcess = new List<string>( 16384 );
		private readonly Settings Settings;
		private readonly Thread ThreadObject;
		private bool _IsBuilding;
		private int _ItemsProcessed;
		private int _ItemsSaved;
		private int _ItemsScanned;
		private int _ItemsToProcess;
		private CatalogProgressStep _ProgressStep;
		private DbConnectionFactory Factory;
		private bool IsRunning;
		private IMessenger MessengerInstance;
		private volatile bool ShouldStop;
		private StackTrace Trace;
	}
}
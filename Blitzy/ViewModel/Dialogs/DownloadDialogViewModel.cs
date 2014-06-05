// $Id$

using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.ViewModel.Dialogs
{
	internal class DownloadDialogViewModel : ViewModelBaseEx
	{
		#region Constructor

		#endregion Constructor

		#region Methods

		public async void StartDownload()
		{
			using( HttpClient client = new HttpClient() )
			{
				LogInfo( "Start download of {0}", DownloadLink );

				HttpResponseMessage response = await client.GetAsync( DownloadLink, HttpCompletionOption.ResponseHeadersRead );
				try
				{
					response.EnsureSuccessStatusCode();
				}
				catch( HttpRequestException ex )
				{
					LogWarning( "Failed to download file: {0}", ex );
					MessengerInstance.Send<DownloadStatusMessage>( new DownloadStatusMessage( TargetPath ), MessageTokens.DownloadFailed );
				}

				Stream responseStream = await response.Content.ReadAsStreamAsync();

				await Task.Run( () =>
				{
					using( FileStream fileStream = File.OpenWrite( TargetPath ) )
					{
						ProgressStatistic stats = new ProgressStatistic();
						stats.UsedEstimatingMethod = ProgressStatistic.EstimatingMethod.CurrentBytesPerSecond;

						long totalLength = DownloadSize;
						if( response.Content.Headers.ContentLength.HasValue )
						{
							totalLength = response.Content.Headers.ContentLength.Value;
						}

						LogInfo( "Download size: {0} bytes", totalLength );

						DownloadSize = totalLength;
						stats.ProgressChanged += stats_ProgressChanged;
						stats.Finished += stats_Finished;

						CopyArguments = new CopyFromArguments( stats.ProgressChange, TimeSpan.FromSeconds( 0.5 ), totalLength );
						DispatcherHelper.CheckBeginInvokeOnUI( () => System.Windows.Input.CommandManager.InvalidateRequerySuggested() );
						fileStream.CopyFrom( responseStream, CopyArguments );
						stats.Finish();

						LogInfo( "Download completed" );
					}
				} );
			}
		}

		private void stats_Finished( object sender, ProgressEventArgs e )
		{
			if( CopyArguments.StopEvent != null )
			{
				LogInfo( "User cancelled download" );
				// User cancelled operation. Don't compute hashes
				try
				{
					File.Delete( TargetPath );
				}
				catch( IOException )
				{
					// Temporary file... Windows will take care of this
					LogWarning( "Failed to delete temporarry file {0}", TargetPath );
				}
			}
			else
			{
				CopyArguments = null;
				using( System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create() )
				{
					using( FileStream stream = File.OpenRead( TargetPath ) )
					{
						string computedHash = BitConverter.ToString( md5.ComputeHash( stream ) ).Replace( "-", "" ).ToLower();

						if( !computedHash.Equals( MD5, StringComparison.Ordinal ) )
						{
							LogError( "Downloaded file is corrupted. Exepected Hash: {0} - Calculated: {1}", MD5, computedHash );
							MessengerInstance.Send<DownloadStatusMessage>( new DownloadStatusMessage( TargetPath ), MessageTokens.DownloadCorrupted );
						}
					}
				}

				MessengerInstance.Send<DownloadStatusMessage>( new DownloadStatusMessage( TargetPath ), MessageTokens.DownloadSucessful );
			}

			DispatcherHelper.CheckBeginInvokeOnUI( () => Close() );
		}

		private void stats_ProgressChanged( object sender, ProgressEventArgs e )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
			{
				TimeLeft = e.ProgressStatistic.EstimatedFinishingTime - DateTime.Now;
				BytesDownloaded = e.ProgressStatistic.BytesRead;
			} );
		}

		#endregion Methods

		#region Commands

		private RelayCommand _CancelCommand;

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
			}
		}

		private bool CanExecuteCancelCommand()
		{
			return CopyArguments != null;
		}

		private void ExecuteCancelCommand()
		{
			ManualResetEvent evt = new ManualResetEvent( true );
			CopyArguments.StopEvent = evt;
		}

		#endregion Commands

		#region Properties

		private long _BytesDownloaded;
		private string _DownloadLink;
		private long _DownloadSize;
		private string _MD5;
		private string _TargetPath;
		private TimeSpan _TimeLeft;

		public long BytesDownloaded
		{
			get
			{
				return _BytesDownloaded;
			}

			set
			{
				if( _BytesDownloaded == value )
				{
					return;
				}

				RaisePropertyChanging( () => BytesDownloaded );
				_BytesDownloaded = value;
				RaisePropertyChanged( () => BytesDownloaded );
			}
		}

		public string DownloadLink
		{
			get
			{
				return _DownloadLink;
			}

			set
			{
				if( _DownloadLink == value )
				{
					return;
				}

				RaisePropertyChanging( () => DownloadLink );
				_DownloadLink = value;
				RaisePropertyChanged( () => DownloadLink );
			}
		}

		public long DownloadSize
		{
			get
			{
				return _DownloadSize;
			}

			set
			{
				if( _DownloadSize == value )
				{
					return;
				}

				RaisePropertyChanging( () => DownloadSize );
				_DownloadSize = value;
				RaisePropertyChanged( () => DownloadSize );
			}
		}

		public string MD5
		{
			get
			{
				return _MD5;
			}

			set
			{
				if( _MD5 == value )
				{
					return;
				}

				RaisePropertyChanging( () => MD5 );
				_MD5 = value;
				RaisePropertyChanged( () => MD5 );
			}
		}

		public string TargetPath
		{
			get
			{
				return _TargetPath;
			}

			set
			{
				if( _TargetPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => TargetPath );
				_TargetPath = value;
				RaisePropertyChanged( () => TargetPath );
			}
		}

		public TimeSpan TimeLeft
		{
			get
			{
				return _TimeLeft;
			}

			set
			{
				if( _TimeLeft == value )
				{
					return;
				}

				RaisePropertyChanging( () => TimeLeft );
				_TimeLeft = value;
				RaisePropertyChanged( () => TimeLeft );
			}
		}

		#endregion Properties

		#region Attributes

		private CopyFromArguments CopyArguments;

		#endregion Attributes
	}
}
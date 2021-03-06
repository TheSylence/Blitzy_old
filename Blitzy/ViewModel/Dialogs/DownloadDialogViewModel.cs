﻿using System;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.CommandWpf;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.ViewModel.Dialogs
{
	internal class DownloadDialogViewModel : ViewModelBaseEx
	{
		public DownloadDialogViewModel( ViewServiceManager serivceManager = null, IMessenger messenger = null )
			: base( null, serivceManager, messenger )
		{
		}

		public async Task StartDownload()
		{
			await Task.Run( async () =>
			{
				using( HttpClient client = new HttpClient() )
				{
					LogInfo( "Start download of {0} (Size: {1})", DownloadLink, DownloadSize );
					ProgressStatistic stats = new ProgressStatistic
					{
						UsedEstimatingMethod = ProgressStatistic.EstimatingMethod.CurrentBytesPerSecond
					};
					CopyArguments = new CopyFromArguments( stats.ProgressChange, TimeSpan.FromSeconds( 0.5 ), 0 );

					HttpResponseMessage response = await client.GetAsync( DownloadLink, HttpCompletionOption.ResponseHeadersRead );
					try
					{
						response.EnsureSuccessStatusCode();
					}
					catch( HttpRequestException ex )
					{
						LogWarning( "Failed to download file: {0}", ex );
						MessengerInstance.Send( new DownloadStatusMessage( TargetPath, DownloadLink, DownloadSize, MD5 ), MessageTokens.DownloadFailed );
						return;
					}

					Stream responseStream = await response.Content.ReadAsStreamAsync();

					using( FileStream fileStream = File.OpenWrite( TargetPath ) )
					{
						long totalLength = DownloadSize;
						if( response.Content.Headers.ContentLength.HasValue )
						{
							totalLength = response.Content.Headers.ContentLength.Value;
						}

						LogInfo( "Download size: {0} bytes", totalLength );

						DownloadSize = totalLength;
						stats.ProgressChanged += stats_ProgressChanged;

						CopyArguments.TotalLength = totalLength;
						DispatcherHelper.CheckBeginInvokeOnUI( CommandManager.InvalidateRequerySuggested );
						fileStream.CopyFrom( responseStream, CopyArguments );
					}

					stats.Finish();
					LogInfo( "Download completed" );
				}

				FinishDownload();
			} );
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<DownloadStatusMessage>( this, MessageTokens.DownloadCorrupted, OnDownloadCorrupted );
			MessengerInstance.Register<DownloadStatusMessage>( this, MessageTokens.DownloadFailed, OnDownloadFailed );
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

		private void FinishDownload()
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
				bool success = true;
				CopyArguments = null;
				using( MD5 md5 = System.Security.Cryptography.MD5.Create() )
				{
					using( FileStream stream = File.OpenRead( TargetPath ) )
					{
						string computedHash = BitConverter.ToString( md5.ComputeHash( stream ) ).Replace( "-", "" ).ToLower();

						if( !computedHash.Equals( MD5, StringComparison.Ordinal ) )
						{
							LogError( "Downloaded file is corrupted. Exepected Hash: {0} - Calculated: {1}", MD5, computedHash );
							MessengerInstance.Send( new DownloadStatusMessage( TargetPath, DownloadLink, DownloadSize, MD5 ), MessageTokens.DownloadCorrupted );
							success = false;
						}
					}
				}

				if( success )
				{
					MessengerInstance.Send( new DownloadStatusMessage( TargetPath, DownloadLink, DownloadSize, MD5 ), MessageTokens.DownloadSucessful );
					DownloadSuccessfull = true;
				}
			}

			DispatcherHelper.CheckBeginInvokeOnUI( () => Close() );
		}

		private void OnDownloadCorrupted( DownloadStatusMessage msg )
		{
			ShowRetryDialog( "DownloadCorruptedRetryQuestion".Localize(), msg );
		}

		private void OnDownloadFailed( DownloadStatusMessage msg )
		{
			ShowRetryDialog( "DownloadFailedRetryQuestion".Localize(), msg );
		}

		private void ShowRetryDialog( string message, DownloadStatusMessage msg )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
			{
				Close();

				MessageBoxParameter args = new MessageBoxParameter( message, "DownloadFailed".Localize(), MessageBoxButton.YesNo, MessageBoxImage.Error );
				MessageBoxResult result = ServiceManagerInstance.Show<MessageBoxService, MessageBoxResult>( args );
				if( result == MessageBoxResult.Yes )
				{
					DownloadServiceParameters downloadArgs = new DownloadServiceParameters( new Uri( msg.DownloadLink ), msg.TargetPath, msg.DownloadSize, msg.MD5 );
					ServiceManagerInstance.Show<DownloadService>( downloadArgs );
				}
			} );
		}

		private void stats_ProgressChanged( object sender, ProgressEventArgs e )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
			{
				TimeLeft = e.ProgressStatistic.EstimatedFinishingTime - DateTime.Now;
				BytesDownloaded = e.ProgressStatistic.BytesRead;
			} );
		}

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

				_BytesDownloaded = value;
				RaisePropertyChanged( () => BytesDownloaded );
			}
		}

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
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

				_DownloadSize = value;
				RaisePropertyChanged( () => DownloadSize );
			}
		}

		public bool DownloadSuccessfull
		{
			get
			{
				return _DownloadSuccessfull;
			}

			set
			{
				if( _DownloadSuccessfull == value )
				{
					return;
				}

				_DownloadSuccessfull = value;
				RaisePropertyChanged( () => DownloadSuccessfull );
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

				_TimeLeft = value;
				RaisePropertyChanged( () => TimeLeft );
			}
		}

		private long _BytesDownloaded;
		private RelayCommand _CancelCommand;
		private string _DownloadLink;
		private long _DownloadSize;
		private bool _DownloadSuccessfull;
		private string _MD5;
		private string _TargetPath;
		private TimeSpan _TimeLeft;

		private CopyFromArguments CopyArguments;
	}
}
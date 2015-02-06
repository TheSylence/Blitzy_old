using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.btbapi;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	internal class ExceptionDialogViewModel : ViewModelBaseEx
	{
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors" )]
		public ExceptionDialogViewModel( Exception ex, StackTrace trace, ViewServiceManager serviceManager = null )
			: base( null, serviceManager )
		{
			ErrorReport = new ErrorReport( ex, trace );
		}

		private bool CanExecuteExitCommand()
		{
			return true;
		}

		private bool CanExecuteSendCommand()
		{
			return true;
		}

		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		private void CloseDialog()
		{
			Close();
			if( !RuntimeConfig.Tests )
			{
				Environment.Exit( -1 );
			}
		}

		private void ExecuteExitCommand()
		{
			CloseDialog();
		}

		private void ExecuteSendCommand()
		{
			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;
			ErrorReportResult result = null;

			Task.Run( async () =>
			{
				result = await API.SendReport( ErrorReport, Constants.SoftwareName, currentVersion );
			} ).Wait();
			if( result.Status != System.Net.HttpStatusCode.OK )
			{
				LogWarning( "Failed to send error report: {0}", result.RawResponse );

				ServiceManagerInstance.Show<MessageBoxService>( new MessageBoxParameter( "ErrorReportError".Localize(),
					"Error".Localize(), MessageBoxButton.OK, MessageBoxImage.Error ) );
			}
			else
			{
				ServiceManagerInstance.Show<MessageBoxService>( new MessageBoxParameter( "ErrorReportSend".Localize(),
					"Success".Localize(), MessageBoxButton.OK, MessageBoxImage.Information ) );
			}

			CloseDialog();
		}

		public ErrorReport ErrorReport
		{
			get
			{
				return _ErrorReport;
			}

			set
			{
				if( _ErrorReport == value )
				{
					return;
				}

				RaisePropertyChanging( () => ErrorReport );
				_ErrorReport = value;
				RaisePropertyChanged( () => ErrorReport );

				if( _ErrorReport != null )
				{
					ErrorReportText = _ErrorReport.ToString();
				}
			}
		}

		public string ErrorReportText
		{
			get
			{
				return _ErrorReportText;
			}

			set
			{
				if( _ErrorReportText == value )
				{
					return;
				}

				RaisePropertyChanging( () => ErrorReportText );
				_ErrorReportText = value;
				RaisePropertyChanged( () => ErrorReportText );
			}
		}

		public RelayCommand ExitCommand
		{
			get
			{
				return _ExitCommand ??
					( _ExitCommand = new RelayCommand( ExecuteExitCommand, CanExecuteExitCommand ) );
			}
		}

		public RelayCommand SendCommand
		{
			get
			{
				return _SendCommand ??
					( _SendCommand = new RelayCommand( ExecuteSendCommand, CanExecuteSendCommand ) );
			}
		}

		private API API
		{
			get
			{
				if( !RuntimeConfig.Tests )
				{
					return new API( APIEndPoint.Default );
				}

				return new API( APIEndPoint.Localhost );
			}
		}

		private ErrorReport _ErrorReport;
		private string _ErrorReportText;
		private RelayCommand _ExitCommand;
		private RelayCommand _SendCommand;
	}
}
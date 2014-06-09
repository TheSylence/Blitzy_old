// $Id$

using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Utility;
using Blitzy.ViewServices;
using btbapi;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	internal class ExceptionDialogViewModel : ViewModelBaseEx
	{
		#region Constructor

		public ExceptionDialogViewModel( Exception ex, StackTrace trace )
		{
			ErrorReport = new ErrorReport( ex, trace );
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Commands

		private RelayCommand _ExitCommand;
		private RelayCommand _SendCommand;

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

		private bool CanExecuteExitCommand()
		{
			return true;
		}

		private bool CanExecuteSendCommand()
		{
			return true;
		}

		private void ExecuteExitCommand()
		{
			Close();
			if( !RuntimeConfig.Tests )
			{
				Environment.Exit( -1 );
			}
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
				DialogServiceManager.Show<MessageBoxService>( new MessageBoxParameter( "ErrorReportError".Localize(),
					"Error".Localize(), MessageBoxButton.OK, MessageBoxImage.Error ) );
			}
			else
			{
				DialogServiceManager.Show<MessageBoxService>( new MessageBoxParameter( "ErrorReportSend".Localize(),
					"Error".Localize(), MessageBoxButton.OK, MessageBoxImage.Information ) );
			}

			Close();
			if( !RuntimeConfig.Tests )
			{
				Environment.Exit( -1 );
			}
		}

		#endregion Commands

		#region Properties

		private ErrorReport _ErrorReport;

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
			}
		}

		private API API
		{
			get
			{
				if( !RuntimeConfig.Tests )
				{
#if DEBUG
					return new API( APIEndPoint.Localhost );
#else
				return new btbapi.API( APIEndPoint.Default );
#endif
				}

				return new API( APIEndPoint.Localhost );
			}
		}

		#endregion Properties
	}
}
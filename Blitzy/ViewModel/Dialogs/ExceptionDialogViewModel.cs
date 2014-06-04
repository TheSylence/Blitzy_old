// $Id$

using System;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	internal class ExceptionDialogViewModel : ViewModelBaseEx
	{
		#region Constructor

		public ExceptionDialogViewModel( Exception ex )
		{
			Ex = ex;
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
			// TODO: Send report

			Close();
			if( !RuntimeConfig.Tests )
			{
				Environment.Exit( -1 );
			}
		}

		#endregion Commands

		#region Properties

		#endregion Properties

		#region Attributes

		private Exception Ex;

		#endregion Attributes
	}
}
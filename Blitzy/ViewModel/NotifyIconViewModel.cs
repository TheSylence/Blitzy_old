// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class NotifyIconViewModel : ViewModelBaseEx
	{
		#region Constructor

		public NotifyIconViewModel()
		{
			ViewModelLocator vmloc = (ViewModelLocator)App.Current.FindResource( "Locator" );
			MainVM = vmloc.Main;
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Commands

		private RelayCommand _QuitCommand;
		private RelayCommand _SettingsCommand;
		private RelayCommand _ShowCommand;

		public RelayCommand QuitCommand
		{
			get
			{
				return _QuitCommand ??
					( _QuitCommand = new RelayCommand( ExecuteQuitCommand, CanExecuteQuitCommand ) );
			}
		}

		public RelayCommand SettingsCommand
		{
			get
			{
				return _SettingsCommand ??
					( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand, CanExecuteSettingsCommand ) );
			}
		}

		public RelayCommand ShowCommand
		{
			get
			{
				return _ShowCommand ??
					( _ShowCommand = new RelayCommand( ExecuteShowCommand, CanExecuteShowCommand ) );
			}
		}

		private bool CanExecuteQuitCommand()
		{
			return true;
		}

		private bool CanExecuteSettingsCommand()
		{
			return MainVM.SettingsCommand.CanExecute( null );
		}

		private bool CanExecuteShowCommand()
		{
			return true;
		}

		private void ExecuteQuitCommand()
		{
			MessengerInstance.Send<InternalCommandMessage>( new InternalCommandMessage( "quit" ) );
		}

		private void ExecuteSettingsCommand()
		{
			MainVM.SettingsCommand.Execute( null );
		}

		private void ExecuteShowCommand()
		{
		}

		#endregion Commands

		#region Properties

		public string Title
		{
			get
			{
#if DEBUG
				return "Blitzy DEBUG";
#else
				return "Blitzy";
#endif
			}
		}

		#endregion Properties

		#region Attributes

		private MainViewModel MainVM;

		#endregion Attributes
	}
}
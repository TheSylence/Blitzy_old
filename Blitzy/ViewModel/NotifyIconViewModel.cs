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

			IconSource = "/Blitzy;component/Resources/TrayIcon.ico";
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<CommandMessage>( this, msg => OnCommand( msg ) );
		}

		#endregion Constructor

		#region Methods

		private void OnCommand( CommandMessage msg )
		{
			switch( msg.Status )
			{
				case CommandStatus.Finished:
					IconSource = "/Blitzy;component/Resources/TrayIcon.ico";
					break;

				case CommandStatus.Executing:
					IconSource = "/Blitzy;component/Resources/CommandExecuting.ico";
					break;

				case CommandStatus.Error:
					IconSource = "/Blitzy;component/Resources/TrayIconFailure.ico";
					break;
			}
		}

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
			MainVM.RaiseShow();
		}

		#endregion Commands

		#region Properties

		private string _IconSource;

		public string IconSource
		{
			get
			{
				return _IconSource;
			}

			set
			{
				if( _IconSource == value )
				{
					return;
				}

				RaisePropertyChanging( () => IconSource );
				_IconSource = value;
				RaisePropertyChanged( () => IconSource );
			}
		}

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

		public bool Visible
		{
			get
			{
				return MainVM.Settings.GetValue<bool>( Model.SystemSetting.TrayIcon );
			}
		}

		#endregion Properties

		#region Attributes

		private MainViewModel MainVM;

		#endregion Attributes
	}
}
// $Id$

using Blitzy.Messages;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class NotifyIconViewModel : ViewModelBaseEx
	{
		#region Constructor

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors" )]
		public NotifyIconViewModel()
		{
			if( !RuntimeConfig.Tests )
			{
				ViewModelLocator vmloc = (ViewModelLocator)App.Current.FindResource( "Locator" );
				MainVM = vmloc.Main;
			}

			_IconSource = "/Blitzy;component/Resources/TrayIcon.ico";
			Reset();
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<CommandMessage>( this, msg => OnCommand( msg ) );
			MessengerInstance.Register<VersionCheckMessage>( this, msg => OnVersionCheck( msg ) );
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

		private void OnVersionCheck( VersionCheckMessage msg )
		{
			bool update = msg.VersionInfo.LatestVersion > msg.CurrentVersion;
			if( update )
			{
				string message = string.Format( "NewVersionAvailable".Localize(), msg.VersionInfo.LatestVersion );
				MessengerInstance.Send<BalloonTipMessage>( new BalloonTipMessage( "UpdateAvailable".Localize(), message, Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info, msg ) );
			}
			else if( msg.ShowIfNewest )
			{
				MessengerInstance.Send<BalloonTipMessage>( new BalloonTipMessage( "VersionUpToDate".Localize(), "VersionUpToDateMessage".Localize(), Hardcodet.Wpf.TaskbarNotification.BalloonIcon.Info ) );
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
				bool visible = MainVM.Settings.GetValue<bool>( Model.SystemSetting.TrayIcon );
				return visible;
			}
		}

		#endregion Properties

		#region Attributes

		internal MainViewModel MainVM;

		#endregion Attributes
	}
}
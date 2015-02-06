using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace Blitzy.ViewModel
{
	internal class NotifyIconViewModel : ViewModelBaseEx
	{
		[SuppressMessage( "Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors" )]
		public NotifyIconViewModel( IMessenger messenger = null )
			: base( null, null, messenger )
		{
			if( !RuntimeConfig.Tests && !IsInDesignMode )
			{
				ViewModelLocator vmloc = (ViewModelLocator)Application.Current.FindResource( "Locator" );
				Debug.Assert( vmloc != null );
				MainVm = vmloc.Main;
			}

			_IconSource = "/Blitzy;component/Resources/TrayIcon.ico";
			Reset();
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<CommandMessage>( this, OnCommand );
			MessengerInstance.Register<VersionCheckMessage>( this, OnVersionCheck );
		}

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
				MessengerInstance.Send( new BalloonTipMessage( "UpdateAvailable".Localize(), message, BalloonIcon.Info, msg ) );
			}
			else if( msg.ShowIfNewest )
			{
				MessengerInstance.Send( new BalloonTipMessage( "VersionUpToDate".Localize(), "VersionUpToDateMessage".Localize(), BalloonIcon.Info ) );
			}
		}

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
			return MainVm.SettingsCommand.CanExecute( null );
		}

		private bool CanExecuteShowCommand()
		{
			return true;
		}

		private void ExecuteQuitCommand()
		{
			MessengerInstance.Send( new InternalCommandMessage( "quit" ) );
		}

		private void ExecuteSettingsCommand()
		{
			MainVm.SettingsCommand.Execute( null );
		}

		private void ExecuteShowCommand()
		{
			MainVm.RaiseShow();
		}

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
				bool visible = MainVm.Settings.GetValue<bool>( SystemSetting.TrayIcon );
				return visible;
			}
		}

		internal MainViewModel MainVm;
	}
}
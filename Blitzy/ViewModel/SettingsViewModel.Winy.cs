using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Plugin.SystemPlugins;

namespace Blitzy.ViewModel
{
	internal class WinySettingsViewModel : SettingsViewModelBase, IPluginViewModel
	{
		public WinySettingsViewModel( DbConnectionFactory connectionFactory, Settings settings, IViewServiceManager serviceManager )
			: base( settings, connectionFactory, serviceManager )
		{
			_LogoffConfirmation = Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.LogoffKey );
			_ShutdownConfirmation = Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.ShutdownKey );
			_RestartConfirmation = Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.RestartKey );
		}

		public void RestoreDefaults()
		{
			throw new System.NotImplementedException();
		}

		public override void Save()
		{
			Settings.SetPluginSetting( Winy.GuidString, Winy.LogoffKey, LogoffConfirmation );
			Settings.SetPluginSetting( Winy.GuidString, Winy.ShutdownKey, ShutdownConfirmation );
			Settings.SetPluginSetting( Winy.GuidString, Winy.RestartKey, RestartConfirmation );
		}

		public bool LogoffConfirmation
		{
			get
			{
				return _LogoffConfirmation;
			}

			set
			{
				if( _LogoffConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => LogoffConfirmation );
				_LogoffConfirmation = value;
				RaisePropertyChanged( () => LogoffConfirmation );
			}
		}

		public bool RestartConfirmation
		{
			get
			{
				return _RestartConfirmation;
			}

			set
			{
				if( _RestartConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => RestartConfirmation );
				_RestartConfirmation = value;
				RaisePropertyChanged( () => RestartConfirmation );
			}
		}

		public bool ShutdownConfirmation
		{
			get
			{
				return _ShutdownConfirmation;
			}

			set
			{
				if( _ShutdownConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShutdownConfirmation );
				_ShutdownConfirmation = value;
				RaisePropertyChanged( () => ShutdownConfirmation );
			}
		}

		private bool _LogoffConfirmation;
		private bool _RestartConfirmation;
		private bool _ShutdownConfirmation;
	}
}
// $Id$

using Blitzy.Plugin.System;

namespace Blitzy.ViewModel
{
	internal class WinySettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public WinySettingsViewModel( SettingsViewModel baseVm )
			: base( baseVm )
		{
			_LogoffConfirmation = BaseVm.Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.LogoffKey );
			_ShutdownConfirmation = BaseVm.Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.ShutdownKey );
			_RestartConfirmation = BaseVm.Settings.GetPluginSetting<bool>( Winy.GuidString, Winy.RestartKey );
		}

		#endregion Constructor

		public override void Save()
		{
			BaseVm.Settings.SetPluginSetting( Winy.GuidString, Winy.LogoffKey, LogoffConfirmation );
			BaseVm.Settings.SetPluginSetting( Winy.GuidString, Winy.ShutdownKey, ShutdownConfirmation );
			BaseVm.Settings.SetPluginSetting( Winy.GuidString, Winy.RestartKey, RestartConfirmation );
		}

		#region Methods

		#endregion Methods

		#region Properties

		private bool _LogoffConfirmation;
		private bool _RestartConfirmation;
		private bool _ShutdownConfirmation;

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

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
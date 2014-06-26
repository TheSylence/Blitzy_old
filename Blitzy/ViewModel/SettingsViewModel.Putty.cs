// $Id$

using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Plugin.SystemPlugins;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class PuttySettingsViewModel : SettingsViewModelBase, IPluginViewModel
	{
		#region Constructor

		public PuttySettingsViewModel( Settings settings )
			: base( settings )
		{
			_PuttyPath = Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey );
			_ImportSessions = Settings.GetPluginSetting<bool>( Putty.GuidString, Putty.ImportKey );
		}

		#endregion Constructor

		#region Methods

		public void RestoreDefaults()
		{
			throw new System.NotImplementedException();
		}

		public override void Save()
		{
			Settings.SetPluginSetting( Putty.GuidString, Putty.PathKey, PuttyPath );
			Settings.SetPluginSetting( Putty.GuidString, Putty.ImportKey, ImportSessions );
		}

		#endregion Methods

		#region Commands

		private RelayCommand _BrowsePuttyCommand;

		public RelayCommand BrowsePuttyCommand
		{
			get
			{
				return _BrowsePuttyCommand ??
					( _BrowsePuttyCommand = new RelayCommand( ExecuteBrowsePuttyCommand, CanExecuteBrowsePuttyCommand ) );
			}
		}

		private bool CanExecuteBrowsePuttyCommand()
		{
			return true;
		}

		private void ExecuteBrowsePuttyCommand()
		{
			FileDialogParameters args = new FileDialogParameters( "ExeFileFilter".Localize() );
			string fileName = DialogServiceManager.Show<OpenFileService, string>( args );
			if( string.IsNullOrWhiteSpace( fileName ) )
			{
				return;
			}

			PuttyPath = fileName;
		}

		#endregion Commands

		#region Properties

		private bool _ImportSessions;
		private string _PuttyPath;

		public bool ImportSessions
		{
			get
			{
				return _ImportSessions;
			}

			set
			{
				if( _ImportSessions == value )
				{
					return;
				}

				RaisePropertyChanging( () => ImportSessions );
				_ImportSessions = value;
				RaisePropertyChanged( () => ImportSessions );
			}
		}

		public string PuttyPath
		{
			get
			{
				return _PuttyPath;
			}

			set
			{
				if( _PuttyPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => PuttyPath );
				_PuttyPath = value;
				RaisePropertyChanged( () => PuttyPath );
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
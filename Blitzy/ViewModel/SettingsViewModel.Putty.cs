// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin.System;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class PuttySettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public PuttySettingsViewModel( SettingsViewModel baseVM )
			: base( baseVM )
		{
			_PuttyPath = BaseVM.Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey );
			_ImportSessions = BaseVM.Settings.GetPluginSetting<bool>( Putty.GuidString, Putty.ImportKey );
		}

		#endregion Constructor

		#region Methods

		public override void Save()
		{
			BaseVM.Settings.SetPluginSetting( Putty.GuidString, Putty.PathKey, PuttyPath );
			BaseVM.Settings.SetPluginSetting( Putty.GuidString, Putty.ImportKey, ImportSessions );
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
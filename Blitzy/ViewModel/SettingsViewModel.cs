// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Model;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class SettingsViewModel : ViewModelBaseEx
	{
		#region Constructor

		public SettingsViewModel()
		{
			CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString();

			using( TextReader reader = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "Blitzy.Resources.Docs.Changelog.txt" ) ) )
			{
				Changelog = reader.ReadToEnd();
			}

			WebySettings = new WebySettingsViewModel( this );
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Commands

		private RelayCommand _AddExcludeCommand;
		private RelayCommand _AddFolderCommand;
		private RelayCommand _AddRuleCommand;
		private RelayCommand _CancelCommand;
		private RelayCommand _DefaultsCommand;
		private RelayCommand _RemoveExcludeCommand;
		private RelayCommand _RemoveFolderCommand;
		private RelayCommand _RemoveRuleCommand;
		private RelayCommand _SaveCommand;
		private RelayCommand _UpdateCatalogCommand;
		private RelayCommand _UpdateCheckCommand;

		public RelayCommand AddExcludeCommand
		{
			get
			{
				return _AddExcludeCommand ??
					( _AddExcludeCommand = new RelayCommand( ExecuteAddExcludeCommand, CanExecuteAddExcludeCommand ) );
			}
		}

		public RelayCommand AddFolderCommand
		{
			get
			{
				return _AddFolderCommand ??
					( _AddFolderCommand = new RelayCommand( ExecuteAddFolderCommand, CanExecuteAddFolderCommand ) );
			}
		}

		public RelayCommand AddRuleCommand
		{
			get
			{
				return _AddRuleCommand ??
					( _AddRuleCommand = new RelayCommand( ExecuteAddRuleCommand, CanExecuteAddRuleCommand ) );
			}
		}

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
			}
		}

		public RelayCommand DefaultsCommand
		{
			get
			{
				return _DefaultsCommand ??
					( _DefaultsCommand = new RelayCommand( ExecuteDefaultsCommand, CanExecuteDefaultsCommand ) );
			}
		}

		public RelayCommand RemoveExcludeCommand
		{
			get
			{
				return _RemoveExcludeCommand ??
					( _RemoveExcludeCommand = new RelayCommand( ExecuteRemoveExcludeCommand, CanExecuteRemoveExcludeCommand ) );
			}
		}

		public RelayCommand RemoveFolderCommand
		{
			get
			{
				return _RemoveFolderCommand ??
					( _RemoveFolderCommand = new RelayCommand( ExecuteRemoveFolderCommand, CanExecuteRemoveFolderCommand ) );
			}
		}

		public RelayCommand RemoveRuleCommand
		{
			get
			{
				return _RemoveRuleCommand ??
					( _RemoveRuleCommand = new RelayCommand( ExecuteRemoveRuleCommand, CanExecuteRemoveRuleCommand ) );
			}
		}

		public RelayCommand SaveCommand
		{
			get
			{
				return _SaveCommand ??
					( _SaveCommand = new RelayCommand( ExecuteSaveCommand, CanExecuteSaveCommand ) );
			}
		}

		public RelayCommand UpdateCatalogCommand
		{
			get
			{
				return _UpdateCatalogCommand ??
					( _UpdateCatalogCommand = new RelayCommand( ExecuteUpdateCatalogCommand, CanExecuteUpdateCatalogCommand ) );
			}
		}

		public RelayCommand UpdateCheckCommand
		{
			get
			{
				return _UpdateCheckCommand ??
					( _UpdateCheckCommand = new RelayCommand( ExecuteUpdateCheckCommand, CanExecuteUpdateCheckCommand ) );
			}
		}

		private bool CanExecuteAddExcludeCommand()
		{
			return SelectedFolder != null;
		}

		private bool CanExecuteAddFolderCommand()
		{
			return true;
		}

		private bool CanExecuteAddRuleCommand()
		{
			return SelectedFolder != null;
		}

		private bool CanExecuteCancelCommand()
		{
			return true;
		}

		private bool CanExecuteDefaultsCommand()
		{
			return true;
		}

		private bool CanExecuteRemoveExcludeCommand()
		{
			return SelectedFolder != null && SelectedExclude != null;
		}

		private bool CanExecuteRemoveFolderCommand()
		{
			return SelectedFolder != null;
		}

		private bool CanExecuteRemoveRuleCommand()
		{
			return SelectedFolder != null && SelectedRule != null;
		}

		private bool CanExecuteSaveCommand()
		{
			return true;
		}

		private bool CanExecuteUpdateCatalogCommand()
		{
			return Settings != null && Settings.Folders.Count > 0;
		}

		private bool CanExecuteUpdateCheckCommand()
		{
			return true;
		}

		private void ExecuteAddExcludeCommand()
		{
			TextInputParameter args = new TextInputParameter( "Enter the exlude that should be added. Wildcards (*) are supported", "Add exclude" );
			string exclude = DialogServiceManager.Show<TextInputService, string>( args );

			if( exclude != null )
			{
				SelectedFolder.Excludes.Add( exclude );
			}
		}

		private void ExecuteAddFolderCommand()
		{
			string path = DialogServiceManager.Show<SelectFolderService, string>();
			if( path == null )
			{
				return;
			}

			int id = 1;
			if( Settings.Folders.Count > 0 )
			{
				id = Settings.Folders.Max( f => f.ID ) + 1;
			}

			Settings.Folders.Add( new Folder() { Path = path, ID = id } );
		}

		private void ExecuteAddRuleCommand()
		{
			TextInputParameter args = new TextInputParameter( "Enter the rule that should be added. Wildcards (*) are supported", "Add rule" );
			string rule = DialogServiceManager.Show<TextInputService, string>( args );

			if( rule != null )
			{
				SelectedFolder.Rules.Add( rule );
			}
		}

		private void ExecuteCancelCommand()
		{
			Close();
		}

		private void ExecuteDefaultsCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "Do you really want to revert to the default settings?", "Restore defaults" );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );

			if( result == MessageBoxResult.Yes )
			{
				Settings.SetDefaults();
			}
		}

		private void ExecuteRemoveExcludeCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "Do you really want to remove the selected exlude?", "Remove exlude" );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );

			if( result == MessageBoxResult.Yes )
			{
				SelectedFolder.Excludes.Remove( SelectedExclude );
				SelectedExclude = null;
			}
		}

		private void ExecuteRemoveFolderCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "Do you really want to remove the selected folder?", "Remove folder" );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );

			if( result == MessageBoxResult.Yes )
			{
				Settings.Folders.Remove( SelectedFolder );
				SelectedFolder = null;
			}
		}

		private void ExecuteRemoveRuleCommand()
		{
			MessageBoxParameter args = new MessageBoxParameter( "Do you really want to remove the selected rule?", "Remove rule" );
			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );

			if( result == MessageBoxResult.Yes )
			{
				SelectedFolder.Rules.Remove( SelectedRule );
				SelectedRule = null;
			}
		}

		private void ExecuteSaveCommand()
		{
			Settings.Save();
			Close();
		}

		private void ExecuteUpdateCatalogCommand()
		{
			throw new NotImplementedException();
		}

		private void ExecuteUpdateCheckCommand()
		{
			throw new NotImplementedException();
		}

		#endregion Commands

		#region Properties

		private string _LatestVersion;
		private string _SelectedExclude;
		private Folder _SelectedFolder;
		private string _SelectedRule;
		private Settings _Settings;

		public string Changelog { get; private set; }

		public string CurrentVersion { get; private set; }

		public string LatestVersion
		{
			get
			{
				return _LatestVersion;
			}

			set
			{
				if( _LatestVersion == value )
				{
					return;
				}

				RaisePropertyChanging( () => LatestVersion );
				_LatestVersion = value;
				RaisePropertyChanged( () => LatestVersion );
			}
		}

		public string SelectedExclude
		{
			get
			{
				return _SelectedExclude;
			}

			set
			{
				if( _SelectedExclude == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedExclude );
				_SelectedExclude = value;
				RaisePropertyChanged( () => SelectedExclude );
			}
		}

		public Folder SelectedFolder
		{
			get
			{
				return _SelectedFolder;
			}

			set
			{
				if( _SelectedFolder == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedFolder );
				_SelectedFolder = value;
				RaisePropertyChanged( () => SelectedFolder );
			}
		}

		public string SelectedRule
		{
			get
			{
				return _SelectedRule;
			}

			set
			{
				if( _SelectedRule == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedRule );
				_SelectedRule = value;
				RaisePropertyChanged( () => SelectedRule );
			}
		}

		public Settings Settings
		{
			get
			{
				return _Settings;
			}

			set
			{
				if( _Settings == value )
				{
					return;
				}

				RaisePropertyChanging( () => Settings );
				_Settings = value;
				RaisePropertyChanged( () => Settings );
			}
		}

		public WebySettingsViewModel WebySettings { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
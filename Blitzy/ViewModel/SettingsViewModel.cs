// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Utility;
using Blitzy.ViewServices;
using btbapi;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using CommandManager = System.Windows.Input.CommandManager;

namespace Blitzy.ViewModel
{
	internal class SettingsViewModel : ViewModelBaseEx
	{
		#region Constructor

		public SettingsViewModel()
		{
			FoldersToRemove = new List<Folder>();
			CurrentVersion = Assembly.GetExecutingAssembly().GetName().Version;

			using( TextReader reader = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "Blitzy.Resources.Docs.Changelog.txt" ) ) )
			{
				Changelog = reader.ReadToEnd();
			}

			using( TextReader reader = new StreamReader( Assembly.GetExecutingAssembly().GetManifestResourceStream( "Blitzy.Resources.Docs.License.txt" ) ) )
			{
				BlitzyLicense = reader.ReadToEnd();
			}

			_BuildDate = Assembly.GetExecutingAssembly().LinkerTimestamp();
			_CatalogItemsProcessed = -1;

			AvailableLanguages = new ObservableCollection<CultureInfo>( WPFLocalizeExtension.Providers.ResxLocalizationProvider.Instance.AvailableCultures.Where( c => !string.IsNullOrWhiteSpace( c.IetfLanguageTag ) ) );
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<CatalogStatusMessage>( this, OnCatalogStatusUpdate );
		}

		#endregion Constructor

		#region Methods

		public override void Reset()
		{
			base.Reset();

			WebySettings = new WebySettingsViewModel( this );
			RaisePropertyChanged( () => WebySettings );

			WinySettings = new WinySettingsViewModel( this );
			RaisePropertyChanged( () => WinySettings );

			PuttySettings = new PuttySettingsViewModel( this );
			RaisePropertyChanged( () => PuttySettings );

			WorkspaceSettings = new WorkspaceSettingsViewModel( this );
			RaisePropertyChanged( () => WorkspaceSettings );

			UpdateCheck = Settings.GetValue<bool>( SystemSetting.AutoUpdate );
			StayOnTop = Settings.GetValue<bool>( SystemSetting.StayOnTop );
			TrayIcon = Settings.GetValue<bool>( SystemSetting.TrayIcon );
			CloseOnCommand = Settings.GetValue<bool>( SystemSetting.CloseAfterCommand );
			CloseOnEscape = Settings.GetValue<bool>( SystemSetting.CloseOnEscape );
			CloseOnFocusLost = Settings.GetValue<bool>( SystemSetting.CloseOnFocusLost );
			KeepInput = Settings.GetValue<bool>( SystemSetting.KeepCommand );
			PeriodicallyRebuild = Settings.GetValue<int>( SystemSetting.AutoCatalogRebuild ) > 0;
			RebuildOnChange = Settings.GetValue<bool>( SystemSetting.RebuildCatalogOnChanges );
			RebuildTime = Settings.GetValue<int>( SystemSetting.AutoCatalogRebuild );
			BackupShortcuts = Settings.GetValue<bool>( SystemSetting.BackupShortcuts );
			HistoryCount = Settings.GetValue<int>( SystemSetting.HistoryCount );
			LastCatalogBuild = Settings.GetValue<DateTime>( SystemSetting.LastCatalogBuild );
			ItemsInCatalog = GetItemCount();
			SelectedLanguage = CultureInfo.CreateSpecificCulture( Settings.GetValue<string>( SystemSetting.Language ) );

			while( !AvailableLanguages.Contains( SelectedLanguage ) && SelectedLanguage != null )
			{
				SelectedLanguage = SelectedLanguage.Parent;
			}
		}

		private int GetItemCount()
		{
			using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT COUNT(*) FROM files";

				return Convert.ToInt32( cmd.ExecuteScalar() );
			}
		}

		private void OnCatalogStatusUpdate( CatalogStatusMessage msg )
		{
			if( msg.Status == CatalogStatus.BuildStarted )
			{
				IsCatalogBuilding = true;
			}
			else if( msg.Status == CatalogStatus.BuildFinished )
			{
				Settings.SetValue( SystemSetting.LastCatalogBuild, DateTime.Now );
				LastCatalogBuild = DateTime.Now;
				FilesProcessed = string.Empty;
				IsCatalogBuilding = false;
				CatalogItemsProcessed = -1;
				ItemsInCatalog = CatalogBuilder.ItemsToProcess;
			}
			else if( msg.Status == CatalogStatus.ProgressUpdated )
			{
				if( CatalogBuilder.ProgressStep == CatalogProgressStep.Parsing )
				{
					FilesProcessed = "ParsingCatalogProgress".FormatLocalized( CatalogBuilder.ItemsProcessed, CatalogBuilder.ItemsToProcess );
					CatalogItemsProcessed = CatalogBuilder.ItemsProcessed;
				}
				else
				{
					FilesProcessed = "SavingCatalogProgress".FormatLocalized( CatalogBuilder.ItemsSaved, CatalogBuilder.ItemsToProcess );
					CatalogItemsProcessed = CatalogBuilder.ItemsSaved;
				}
			}

			CommandManager.InvalidateRequerySuggested();
		}

		private void SetLanguage( CultureInfo culture )
		{
			MessengerInstance.Send( new LanguageMessage( culture ) );
		}

		#endregion Methods

		#region Commands

		private RelayCommand _AddExcludeCommand;
		private RelayCommand _AddFolderCommand;
		private RelayCommand _AddRuleCommand;
		private RelayCommand _CancelCommand;
		private RelayCommand _DefaultsCommand;
		private RelayCommand _DownloadUpdateCommand;
		private RelayCommand _RemoveExcludeCommand;
		private RelayCommand _RemoveFolderCommand;
		private RelayCommand _RemoveRuleCommand;
		private RelayCommand _SaveCommand;
		private RelayCommand _UpdateCatalogCommand;
		private RelayCommand _UpdateCheckCommand;
		private RelayCommand _ViewChangelogCommand;

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

		public RelayCommand DownloadUpdateCommand
		{
			get
			{
				return _DownloadUpdateCommand ??
					( _DownloadUpdateCommand = new RelayCommand( ExecuteDownloadUpdateCommand, CanExecuteDownloadUpdateCommand ) );
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

		public RelayCommand ViewChangelogCommand
		{
			get
			{
				return _ViewChangelogCommand ??
					( _ViewChangelogCommand = new RelayCommand( ExecuteViewChangelogCommand, CanExecuteViewChangelogCommand ) );
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

		private bool CanExecuteDownloadUpdateCommand()
		{
			return LatestVersionInfo != null && LatestVersionInfo.DownloadLink != null;
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
			return Settings != null && Settings.Folders.Count > 0 && CatalogBuilder != null && !CatalogBuilder.IsBuilding;
		}

		private bool CanExecuteUpdateCheckCommand()
		{
			return true;
		}

		private bool CanExecuteViewChangelogCommand()
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

			Settings.Folders.Add( new Folder { Path = path, ID = id } );
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

		private void ExecuteDownloadUpdateCommand()
		{
			DownloadServiceParameters args = new DownloadServiceParameters( LatestVersionInfo.DownloadLink, "test.exe", 0, "234" );
			DialogServiceManager.Show<DownloadService>( args );
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
				FoldersToRemove.Add( SelectedFolder );
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
			foreach( Folder folder in FoldersToRemove )
			{
				folder.Delete( Settings.Connection );
			}

			Settings.SetValue( SystemSetting.AutoUpdate, UpdateCheck );
			Settings.SetValue( SystemSetting.StayOnTop, StayOnTop );
			Settings.SetValue( SystemSetting.TrayIcon, TrayIcon );
			Settings.SetValue( SystemSetting.CloseAfterCommand, CloseOnCommand );
			Settings.SetValue( SystemSetting.CloseOnEscape, CloseOnEscape );
			Settings.SetValue( SystemSetting.CloseOnFocusLost, CloseOnFocusLost );
			Settings.SetValue( SystemSetting.KeepCommand, KeepInput );
			Settings.SetValue( SystemSetting.AutoCatalogRebuild, RebuildTime );
			Settings.SetValue( SystemSetting.RebuildCatalogOnChanges, RebuildOnChange );
			Settings.SetValue( SystemSetting.BackupShortcuts, BackupShortcuts );
			Settings.SetValue( SystemSetting.HistoryCount, HistoryCount );
			Settings.SetValue( SystemSetting.Language, SelectedLanguage.IetfLanguageTag );

			Settings.Save();
			WinySettings.Save();
			WebySettings.Save();
			WorkspaceSettings.Save();

			Close();
		}

		private void ExecuteUpdateCatalogCommand()
		{
			MessengerInstance.Send( new InternalCommandMessage( "catalog" ) );
		}

		private void ExecuteUpdateCheckCommand()
		{
			Task.Run( async () =>
			{
				LatestVersionInfo = await UpdateChecker.Instance.CheckVersion();
				if( LatestVersionInfo.Status == HttpStatusCode.OK )
				{
					DispatcherHelper.CheckBeginInvokeOnUI( CommandManager.InvalidateRequerySuggested );
				}
				else
				{
					VersionCheckError = true;
				}
			} );
		}

		private void ExecuteViewChangelogCommand()
		{
			DialogServiceManager.Show<ViewChangelogService>( LatestVersionInfo );
		}

		#endregion Commands

		#region Properties

		#region SettingItems

		private bool _BackupShortcuts;
		private DateTime _BuildDate;
		private bool _CloseOnCommand;
		private bool _CloseOnEscape;
		private bool _CloseOnFocusLost;
		private int _HistoryCount;
		private bool _KeepInput;
		private bool _PeriodicallyRebuild;
		private bool _RebuildOnChange;
		private int _RebuildTime;
		private bool _StayOnTop;
		private bool _TrayIcon;
		private bool _UpdateCheck;

		public bool BackupShortcuts
		{
			get
			{
				return _BackupShortcuts;
			}

			set
			{
				if( _BackupShortcuts == value )
				{
					return;
				}

				RaisePropertyChanging( () => BackupShortcuts );
				_BackupShortcuts = value;
				RaisePropertyChanged( () => BackupShortcuts );
			}
		}

		public DateTime BuildDate
		{
			get
			{
				return _BuildDate;
			}

			set
			{
				if( _BuildDate == value )
				{
					return;
				}

				RaisePropertyChanging( () => BuildDate );
				_BuildDate = value;
				RaisePropertyChanged( () => BuildDate );
			}
		}

		public bool CloseOnCommand
		{
			get
			{
				return _CloseOnCommand;
			}

			set
			{
				if( _CloseOnCommand == value )
				{
					return;
				}

				RaisePropertyChanging( () => CloseOnCommand );
				_CloseOnCommand = value;
				RaisePropertyChanged( () => CloseOnCommand );
			}
		}

		public bool CloseOnEscape
		{
			get
			{
				return _CloseOnEscape;
			}

			set
			{
				if( _CloseOnEscape == value )
				{
					return;
				}

				RaisePropertyChanging( () => CloseOnEscape );
				_CloseOnEscape = value;
				RaisePropertyChanged( () => CloseOnEscape );
			}
		}

		public bool CloseOnFocusLost
		{
			get
			{
				return _CloseOnFocusLost;
			}

			set
			{
				if( _CloseOnFocusLost == value )
				{
					return;
				}

				RaisePropertyChanging( () => CloseOnFocusLost );
				_CloseOnFocusLost = value;
				RaisePropertyChanged( () => CloseOnFocusLost );
			}
		}

		public int HistoryCount
		{
			get
			{
				return _HistoryCount;
			}

			set
			{
				if( _HistoryCount == value )
				{
					return;
				}

				RaisePropertyChanging( () => HistoryCount );
				_HistoryCount = value;
				RaisePropertyChanged( () => HistoryCount );
			}
		}

		public bool KeepInput
		{
			get
			{
				return _KeepInput;
			}

			set
			{
				if( _KeepInput == value )
				{
					return;
				}

				RaisePropertyChanging( () => KeepInput );
				_KeepInput = value;
				RaisePropertyChanged( () => KeepInput );
			}
		}

		public bool PeriodicallyRebuild
		{
			get
			{
				return _PeriodicallyRebuild;
			}

			set
			{
				if( _PeriodicallyRebuild == value )
				{
					return;
				}

				RaisePropertyChanging( () => PeriodicallyRebuild );
				_PeriodicallyRebuild = value;
				RaisePropertyChanged( () => PeriodicallyRebuild );
			}
		}

		public bool RebuildOnChange
		{
			get
			{
				return _RebuildOnChange;
			}

			set
			{
				if( _RebuildOnChange == value )
				{
					return;
				}

				RaisePropertyChanging( () => RebuildOnChange );
				_RebuildOnChange = value;
				RaisePropertyChanged( () => RebuildOnChange );
			}
		}

		public int RebuildTime
		{
			get
			{
				return _RebuildTime;
			}

			set
			{
				if( _RebuildTime == value )
				{
					return;
				}

				RaisePropertyChanging( () => RebuildTime );
				_RebuildTime = value;
				RaisePropertyChanged( () => RebuildTime );
			}
		}

		public bool StayOnTop
		{
			get
			{
				return _StayOnTop;
			}

			set
			{
				if( _StayOnTop == value )
				{
					return;
				}

				RaisePropertyChanging( () => StayOnTop );
				_StayOnTop = value;
				RaisePropertyChanged( () => StayOnTop );
			}
		}

		public bool TrayIcon
		{
			get
			{
				return _TrayIcon;
			}

			set
			{
				if( _TrayIcon == value )
				{
					return;
				}

				RaisePropertyChanging( () => TrayIcon );
				_TrayIcon = value;
				RaisePropertyChanged( () => TrayIcon );
			}
		}

		public bool UpdateCheck
		{
			get
			{
				return _UpdateCheck;
			}

			set
			{
				if( _UpdateCheck == value )
				{
					return;
				}

				RaisePropertyChanging( () => UpdateCheck );
				_UpdateCheck = value;
				RaisePropertyChanged( () => UpdateCheck );
			}
		}

		#endregion SettingItems

		private CatalogBuilder _CatalogBuilder;
		private int _CatalogItemsProcessed;
		private string _FilesProcessed;
		private bool _IsCatalogBuilding;
		private bool _IsNewerVersionAvailable;
		private int _ItemsInCatalog;
		private DateTime _LastCatalogBuild;
		private VersionInfo _LatestVersionInfo;
		private string _SelectedExclude;
		private Folder _SelectedFolder;
		private CultureInfo _SelectedLanguage;
		private string _SelectedRule;
		private Settings _Settings;
		private bool _VersionCheckError;

		public API API { get; private set; }

		public PluginDatabase ApiDatabase { get; set; }

		public ObservableCollection<CultureInfo> AvailableLanguages { get; private set; }

		public string BlitzyLicense { get; set; }

		public CatalogBuilder CatalogBuilder
		{
			get
			{
				return _CatalogBuilder;
			}

			set
			{
				if( _CatalogBuilder == value )
				{
					return;
				}

				RaisePropertyChanging( () => CatalogBuilder );
				_CatalogBuilder = value;
				RaisePropertyChanged( () => CatalogBuilder );
			}
		}

		public int CatalogItemsProcessed
		{
			get
			{
				return _CatalogItemsProcessed;
			}

			set
			{
				if( _CatalogItemsProcessed == value )
				{
					return;
				}

				RaisePropertyChanging( () => CatalogItemsProcessed );
				_CatalogItemsProcessed = value;
				RaisePropertyChanged( () => CatalogItemsProcessed );
			}
		}

		public string Changelog { get; set; }

		public Version CurrentVersion { get; set; }

		public string FilesProcessed
		{
			get
			{
				return _FilesProcessed;
			}

			set
			{
				if( _FilesProcessed == value )
				{
					return;
				}

				RaisePropertyChanging( () => FilesProcessed );
				_FilesProcessed = value;
				RaisePropertyChanged( () => FilesProcessed );
			}
		}

		public bool IsCatalogBuilding
		{
			get
			{
				return _IsCatalogBuilding;
			}

			set
			{
				if( _IsCatalogBuilding == value )
				{
					return;
				}

				RaisePropertyChanging( () => IsCatalogBuilding );
				_IsCatalogBuilding = value;
				RaisePropertyChanged( () => IsCatalogBuilding );
			}
		}

		public bool IsNewerVersionAvailable
		{
			get
			{
				return _IsNewerVersionAvailable;
			}

			set
			{
				if( _IsNewerVersionAvailable == value )
				{
					return;
				}

				RaisePropertyChanging( () => IsNewerVersionAvailable );
				_IsNewerVersionAvailable = value;
				RaisePropertyChanged( () => IsNewerVersionAvailable );
			}
		}

		public int ItemsInCatalog
		{
			get
			{
				return _ItemsInCatalog;
			}

			set
			{
				if( _ItemsInCatalog == value )
				{
					return;
				}

				RaisePropertyChanging( () => ItemsInCatalog );
				_ItemsInCatalog = value;
				RaisePropertyChanged( () => ItemsInCatalog );
			}
		}

		public DateTime LastCatalogBuild
		{
			get
			{
				return _LastCatalogBuild;
			}

			set
			{
				if( _LastCatalogBuild == value )
				{
					return;
				}

				RaisePropertyChanging( () => LastCatalogBuild );
				_LastCatalogBuild = value;
				RaisePropertyChanged( () => LastCatalogBuild );
			}
		}

		public VersionInfo LatestVersionInfo
		{
			get
			{
				return _LatestVersionInfo;
			}

			set
			{
				if( _LatestVersionInfo == value )
				{
					return;
				}

				RaisePropertyChanging( () => LatestVersionInfo );
				_LatestVersionInfo = value;
				RaisePropertyChanged( () => LatestVersionInfo );

				IsNewerVersionAvailable = CurrentVersion < LatestVersionInfo.LatestVersion;
			}
		}

		public PuttySettingsViewModel PuttySettings { get; private set; }

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

		public CultureInfo SelectedLanguage
		{
			get
			{
				return _SelectedLanguage;
			}

			set
			{
				if( _SelectedLanguage == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedLanguage );
				_SelectedLanguage = value;
				SetLanguage( value );
				RaisePropertyChanged( () => SelectedLanguage );
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

		public bool VersionCheckError
		{
			get
			{
				return _VersionCheckError;
			}

			set
			{
				if( _VersionCheckError == value )
				{
					return;
				}

				RaisePropertyChanging( () => VersionCheckError );
				_VersionCheckError = value;
				RaisePropertyChanged( () => VersionCheckError );
			}
		}

		public WebySettingsViewModel WebySettings { get; private set; }

		public WinySettingsViewModel WinySettings { get; private set; }

		public WorkspaceSettingsViewModel WorkspaceSettings { get; private set; }

		#endregion Properties

		#region Attributes

		private readonly List<Folder> FoldersToRemove;

		#endregion Attributes
	}
}
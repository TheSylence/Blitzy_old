using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;
using CommandManager = Blitzy.Model.CommandManager;

namespace Blitzy.ViewModel
{
	public sealed class MainViewModel : ViewModelBaseEx, IPluginHost
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			Database = ToDispose( new Database() );
			Settings = new Settings( Database.Connection );

			if( !Database.CheckExistance() )
			{
				Settings.SetDefaults();
			}
			else
			{
				Settings.Load();
			}

			CultureInfo currentLanguage = LanguageHelper.GetLanguage( Settings.GetValue<string>( SystemSetting.Language ) );
			MessengerInstance.Send( new LanguageMessage( currentLanguage ) );

			ApiDatabase = ToDispose( new PluginDatabase( Database.Connection ) );
			Plugins = ToDispose( new PluginManager( this, Database.Connection ) );
			Plugins.LoadPlugins();

			CmdManager = ToDispose( new CommandManager( Database.Connection, Settings, Plugins ) );

			int rebuildTime = Settings.GetValue<int>( SystemSetting.AutoCatalogRebuild );
			if( rebuildTime > 0 )
			{
				RebuildTimer = new DispatcherTimer { Interval = TimeSpan.FromMinutes( rebuildTime ) };
				RebuildTimer.Tick += RebuildTimer_Tick;
				RebuildTimer.Start();
			}

			Builder = ToDispose( new CatalogBuilder( Settings ) );
			History = ToDispose( new HistoryManager( Settings ) );

			Reset();
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<InternalCommandMessage>( this, msg => OnInternalCommand( msg.Command ) );
			MessengerInstance.Register<CommandMessage>( this, OnCommand );
		}

		#endregion Constructor

		#region Methods

		public override void Cleanup()
		{
			History.Save();

			base.Cleanup();
		}

		internal void RaiseShow()
		{
			Show();
		}

		private void BuildCatalog()
		{
			if( Builder.IsBuilding )
			{
				Builder.Stop();
			}

			if( RebuildTimer != null )
			{
				RebuildTimer.Stop();
				RebuildTimer.Start();
			}

			Builder.Build();
		}

		private void OnCommand( CommandMessage msg )
		{
			if( msg.TaskID.HasValue )
			{
				if( msg.Status != CommandStatus.Executing )
				{
					lock( TaskListLock )
					{
						TaskList.RemoveWhere( id => id == msg.TaskID.Value );
					}
				}
				else
				{
					lock( TaskListLock )
					{
						TaskList.Add( msg.TaskID.Value );
					}
				}
			}
		}

		private void OnInternalCommand( string command )
		{
			switch( command )
			{
				case "quit":
					ShouldClose = true;
					DispatcherHelper.CheckBeginInvokeOnUI( () => Close( true ) );
					break;

				case "catalog":
					BuildCatalog();
					break;

				case "version":
					Task.Run( async () => await UpdateChecker.Instance.CheckVersion( true ) );
					break;

				case "reset":
					CmdManager.ResetExecutionCount();
					break;

				case "history":
					History.Clear();
					break;

				default:
					LogInfo( "Unhandled internal command: {0}", command );
					break;
			}
		}

		private void RebuildTimer_Tick( object sender, EventArgs e )
		{
			BuildCatalog();
		}

		private void UpdateCommandInput()
		{
			// When the command string is empty we can stop here
			if( string.IsNullOrWhiteSpace( CommandInput ) )
			{
				CmdManager.Clear();
				return;
			}

			CmdManager.Clear( false );

			CmdManager.SearchItems( CommandInput );
			// Otherwise the tests won't work because the CommandListView is updating this value
			CmdManager.CurrentItem = CmdManager.Items.FirstOrDefault();
			SelectedCommandIndex = 0;

			// Check if the command does provide any info
			if( CmdManager.CurrentItem != null )
			{
				Collection<string> data = new Collection<string>( CmdManager.GetCommandParts( CommandInput ) );
				CommandInfo = CmdManager.CurrentItem.Plugin.GetInfo( data, CmdManager.CurrentItem );
			}
		}

		#endregion Methods

		#region Commands

		private RelayCommand _ExecuteCommand;
		private RelayCommand<KeyEventArgs> _KeyPreviewCommand;
		private RelayCommand<KeyEventArgs> _KeyUpCommand;
		private RelayCommand<MouseButtonEventArgs> _MouseExecuteCommand;
		private RelayCommand<CancelEventArgs> _OnClosingCommand;
		private RelayCommand _OnDeactivatedCommand;
		private RelayCommand _SettingsCommand;

		public RelayCommand ExecuteCommand
		{
			get
			{
				return _ExecuteCommand ??
					( _ExecuteCommand = new RelayCommand( () => ExecuteExecuteCommand(), CanExecuteExecuteCommand ) );
			}
		}

		public RelayCommand<KeyEventArgs> KeyPreviewCommand
		{
			get
			{
				return _KeyPreviewCommand ??
					( _KeyPreviewCommand = new RelayCommand<KeyEventArgs>( ExecuteKeyPreviewCommand, CanExecuteKeyPreviewCommand ) );
			}
		}

		public RelayCommand<KeyEventArgs> KeyUpCommand
		{
			get
			{
				return _KeyUpCommand ??
					( _KeyUpCommand = new RelayCommand<KeyEventArgs>( ExecuteKeyUpCommand, CanExecuteKeyUpCommand ) );
			}
		}

		public RelayCommand<MouseButtonEventArgs> MouseExecuteCommand
		{
			get
			{
				return _MouseExecuteCommand ??
					( _MouseExecuteCommand = new RelayCommand<MouseButtonEventArgs>( ExecuteMouseExecuteCommand, CanExecuteMouseExecuteCommand ) );
			}
		}

		public RelayCommand<CancelEventArgs> OnClosingCommand
		{
			get
			{
				return _OnClosingCommand ??
					( _OnClosingCommand = new RelayCommand<CancelEventArgs>( ExecuteOnClosingCommand ) );
			}
		}

		public RelayCommand OnDeactivatedCommand
		{
			get
			{
				return _OnDeactivatedCommand ??
					( _OnDeactivatedCommand = new RelayCommand( ExecuteOnDeactivatedCommand, CanExecuteOnDeactivatedCommand ) );
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

		internal bool OnKeyBack()
		{
			if( CommandInput.EndsWith( CmdManager.Separator ) )
			{
				CommandInput = CommandInput.Substring( 0, CommandInput.Length - CmdManager.Separator.Length );
				MessengerInstance.Send( new InputCaretPositionMessage( CommandInput.Length ) );

				return true;
			}

			return false;
		}

		internal bool OnKeyDownArrow()
		{
			if( Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ) )
			{
				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Show, History ) );
				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Down ) );
				return true;
			}

			int idx = CmdManager.Items.IndexOf( CmdManager.CurrentItem );
			SelectedCommandIndex = Math.Min( CmdManager.Items.Count, idx + 1 );
			return true;
		}

		internal bool OnKeyEscape()
		{
			if( Settings.GetValue<bool>( SystemSetting.CloseOnEscape ) )
			{
				Hide();
				return true;
			}

			return false;
		}

		internal bool OnKeyReturn()
		{
			CommandExecutionMode mode = CommandExecutionMode.Default;
			if( Keyboard.IsKeyDown( Key.LeftCtrl ) && Keyboard.IsKeyDown( Key.LeftShift )
				|| Keyboard.IsKeyDown( Key.RightCtrl ) && Keyboard.IsKeyDown( Key.RightShift ) )
			{
				mode = CommandExecutionMode.Secondary;
			}

			if( mode != CommandExecutionMode.Secondary && ( Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ) ) )
			{
				string hist = History.SelectedItem;

				CommandInput = hist;

				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Hide ) );
				MessengerInstance.Send( new InputCaretPositionMessage( CommandInput.Length ) );
				return true;
			}

			if( CmdManager.CurrentItem != null )
			{
				ExecuteExecuteCommand( mode );
				return true;
			}

			return false;
		}

		internal bool OnKeyTab()
		{
			if( CmdManager.CurrentItem == null )
			{
				return false;
			}

			List<string> commandParts = new List<string>( CmdManager.GetCommandParts( CommandInput ) );

			// Autocomplete all commands until a command that accepts input data is reached
			CommandItem item = CmdManager.CurrentItem;

			List<CommandItem> itemChain = new List<CommandItem>();
			for( int i = commandParts.Count - 1; i >= 0; --i )
			{
				if( item == null )
					break;

				itemChain.Add( item );
				item = item.Parent;
			}
			itemChain.Reverse();

			for( int i = 0; i < commandParts.Count; ++i )
			{
				commandParts[i] = itemChain[i].Name;

				if( itemChain[i].AcceptsData )
				{
					break;
				}
			}

			int subCommandCount = CmdManager.CurrentItem.Plugin.GetSubCommands( CmdManager.CurrentItem, commandParts ).Count();
			if( subCommandCount > 0 )
			{
				commandParts.Add( string.Empty );
			}
			if( CmdManager.CurrentItem.AcceptsData )
			{
				bool add = commandParts.Count == 1;

				if( add )
				{
					commandParts.Add( string.Empty );
				}
			}

			CommandInput = string.Join( CmdManager.Separator, commandParts );
			MessengerInstance.Send( new InputCaretPositionMessage( CommandInput.Length ) );
			return true;
		}

		internal bool OnKeyUpArrow()
		{
			if( Keyboard.IsKeyDown( Key.LeftCtrl ) || Keyboard.IsKeyDown( Key.RightCtrl ) )
			{
				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Show, History ) );
				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Up ) );
				return true;
			}

			int idx = CmdManager.Items.IndexOf( CmdManager.CurrentItem );
			SelectedCommandIndex = Math.Max( 0, idx - 1 );
			return true;
		}

		private bool CanExecuteExecuteCommand()
		{
			return CmdManager.CurrentItem != null;
		}

		private bool CanExecuteKeyPreviewCommand( KeyEventArgs args )
		{
			return true;
		}

		private bool CanExecuteKeyUpCommand( KeyEventArgs args )
		{
			return true;
		}

		private bool CanExecuteMouseExecuteCommand( MouseButtonEventArgs args )
		{
			return ExecuteCommand.CanExecute( null );
		}

		private bool CanExecuteOnDeactivatedCommand()
		{
			return true;
		}

		private bool CanExecuteSettingsCommand()
		{
			return true;
		}

		private void ExecuteExecuteCommand( CommandExecutionMode mode = CommandExecutionMode.Default )
		{
			Collection<string> commandData = new Collection<string>( CmdManager.GetCommandParts( CommandInput ) );
			CommandItem item = CmdManager.CurrentItem;

			Action taskAction = () =>
			{
				DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CommandMessage( CommandStatus.Executing, null, Task.CurrentId ) ) );
				string msg = null;
				bool result = false;

				try
				{
					result = item.Plugin.ExecuteCommand( item, mode, commandData, out msg );
				}
				catch( Exception ex )
				{
					result = false;
					LogError( "Failed to execute command ({1}): {0}", ex, CommandInput );
					msg = ex.Message;
				}
				finally
				{
					DispatcherHelper.CheckBeginInvokeOnUI( () => MessengerInstance.Send( new CommandMessage( result ? CommandStatus.Finished : CommandStatus.Error, msg, Task.CurrentId ) ) );
				}
			};

			if( RuntimeConfig.Tests )
			{
				taskAction.Invoke();
			}
			else
			{
				Task.Run( taskAction );
			}

			// We want the top command's name for the execution count, so walk up to the top
			CommandItem tmp = item;
			while( tmp.Parent != null )
			{
				tmp = tmp.Parent;
			}

			CmdManager.UpdateExecutionCount( tmp );
			History.AddItem( CommandInput );

			CommandInput = null;
			CommandInfo = null;
			CmdManager.Clear();

			if( Settings.GetValue<bool>( SystemSetting.CloseAfterCommand ) )
			{
				Hide();
			}
		}

		[ExcludeFromCodeCoverage]
		private void ExecuteKeyPreviewCommand( KeyEventArgs args )
		{
			if( args.Key == Key.Escape )
			{
				args.Handled = OnKeyEscape();
			}
			else if( args.Key == Key.Return )
			{
				args.Handled = OnKeyReturn();
			}
			else if( args.Key == Key.Tab )
			{
				args.Handled = OnKeyTab();
			}
			else if( args.Key == Key.Up )
			{
				args.Handled = OnKeyUpArrow();
			}
			else if( args.Key == Key.Down )
			{
				args.Handled = OnKeyDownArrow();
			}
			else if( args.Key == Key.Back )
			{
				args.Handled = OnKeyBack();
			}
		}

		[ExcludeFromCodeCoverage]
		private void ExecuteKeyUpCommand( KeyEventArgs args )
		{
			if( args.Key == Key.LeftCtrl || args.Key == Key.RightCtrl )
			{
				MessengerInstance.Send( new HistoryMessage( HistoryMessageType.Hide ) );
			}
		}

		[ExcludeFromCodeCoverage]
		private void ExecuteMouseExecuteCommand( MouseButtonEventArgs args )
		{
			if( args.ClickCount >= 2 )
			{
				ExecuteCommand.Execute( null );
			}
		}

		private void ExecuteOnClosingCommand( CancelEventArgs args )
		{
			if( !ShouldClose )
			{
				args.Cancel = true;
				Hide();
			}
		}

		private void ExecuteOnDeactivatedCommand()
		{
			if( Settings.GetValue<bool>( SystemSetting.CloseOnFocusLost ) )
			{
				Hide();
				CommandInput = string.Empty;
			}
		}

		private void ExecuteSettingsCommand()
		{
			SettingsServiceParameters args = new SettingsServiceParameters( Settings, Builder, Plugins );
			DialogServiceManager.Show<SettingsService>( args );
		}

		#endregion Commands

		#region Properties

		private string _CommandInfo;
		private string _CommandInput;
		private int _SelectedCommandIndex;

		public CommandManager CmdManager { get; private set; }

		public string CommandInfo
		{
			get
			{
				return _CommandInfo;
			}

			set
			{
				if( _CommandInfo == value )
				{
					return;
				}

				RaisePropertyChanging( () => CommandInfo );
				_CommandInfo = value;
				RaisePropertyChanged( () => CommandInfo );
			}
		}

		public string CommandInput
		{
			get
			{
				return _CommandInput;
			}

			set
			{
				if( _CommandInput == value )
				{
					return;
				}

				RaisePropertyChanging( () => CommandInput );
				_CommandInput = value;
				RaisePropertyChanged( () => CommandInput );

				UpdateCommandInput();
			}
		}

		public HistoryManager History { get; private set; }

		public int SelectedCommandIndex
		{
			get
			{
				return _SelectedCommandIndex;
			}

			set
			{
				if( _SelectedCommandIndex == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedCommandIndex );
				_SelectedCommandIndex = value;
				RaisePropertyChanged( () => SelectedCommandIndex );
			}
		}

		public bool ShouldClose { get; internal set; }

		internal CatalogBuilder Builder { get; private set; }

		internal Database Database { get; private set; }

		internal PluginManager Plugins { get; private set; }

		internal Settings Settings { get; private set; }

		#endregion Properties

		#region Attributes

		internal HashSet<int> TaskList = new HashSet<int>();
		private readonly DispatcherTimer RebuildTimer;
		private readonly object TaskListLock = new object();

		#endregion Attributes

		#region IPluginHost

		private readonly PluginDatabase ApiDatabase;

		IDatabase IPluginHost.Database
		{
			get
			{
				return ApiDatabase;
			}
		}

		ISettings IPluginHost.Settings
		{
			get { return Settings; }
		}

		bool IPluginHost.IsPluginLoaded( Guid id )
		{
			return Plugins.IsLoaded( id );
		}

		#endregion IPluginHost
	}
}
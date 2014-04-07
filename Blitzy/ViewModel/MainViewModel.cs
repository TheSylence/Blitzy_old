using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Threading;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.ViewModel
{
	public class MainViewModel : ViewModelBaseEx, IPluginHost
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

			Plugins = new PluginManager( this, Database.Connection );
			Plugins.LoadPlugins();

			CmdManager = new Blitzy.Model.CommandManager( Database.Connection, Settings, Plugins );

			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}
		}

		protected override void RegisterMessages()
		{
			base.RegisterMessages();

			MessengerInstance.Register<InternalCommandMessage>( this, msg => OnInternalCommand( msg.Command ) );
			MessengerInstance.Register<CommandMessage>( this, msg => OnCommand( msg ) );
		}

		#endregion Constructor

		#region Methods

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
					DispatcherHelper.CheckBeginInvokeOnUI( () => Close( true ) );
					break;
			}
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
			CmdManager.CurrentItem = CmdManager.Items[0];
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

		private RelayCommand _SettingsCommand;

		public RelayCommand ExecuteCommand
		{
			get
			{
				return _ExecuteCommand ??
					( _ExecuteCommand = new RelayCommand( ExecuteExecuteCommand, CanExecuteExecuteCommand ) );
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

		public RelayCommand SettingsCommand
		{
			get
			{
				return _SettingsCommand ??
					( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand, CanExecuteSettingsCommand ) );
			}
		}

		private bool CanExecuteExecuteCommand()
		{
			return true;
		}

		private bool CanExecuteKeyPreviewCommand( KeyEventArgs args )
		{
			return true;
		}

		private bool CanExecuteSettingsCommand()
		{
			return true;
		}

		private void ExecuteExecuteCommand()
		{
			Collection<string> commandData = new Collection<string>( CmdManager.GetCommandParts( CommandInput ) );
			CommandItem item = CmdManager.CurrentItem;

			Action taskAction = () =>
			{
				DispatcherHelper.CheckBeginInvokeOnUI( () =>
					{
						MessengerInstance.Send<CommandMessage>( new CommandMessage( CommandStatus.Executing, null, Task.CurrentId ) );
					} );
				string msg = null;
				bool result = false;

				try
				{
					result = item.Plugin.ExecuteCommand( item, commandData, out msg );
				}
				catch( Exception ex )
				{
					result = false;
					LogError( "Failed to execute command: {0}", ex );
				}
				finally
				{
					DispatcherHelper.CheckBeginInvokeOnUI( () =>
					{
						MessengerInstance.Send<CommandMessage>( new CommandMessage( result ? CommandStatus.Finished : CommandStatus.Error, msg, Task.CurrentId ) );
					} );
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
			string itemName = tmp.Name;

			CmdManager.UpdateExecutionCount( itemName, item.Plugin.PluginID );
			// TODO: History

			CommandInput = null;
			CommandInfo = null;
			CmdManager.Clear();

			if( Settings.GetValue<bool>( SystemSetting.CloseAfterCommand ) )
			{
				Hide();
			}
		}

		private void ExecuteKeyPreviewCommand( KeyEventArgs args )
		{
			if( args.Key == Key.Escape )
			{
				if( Settings.GetValue<bool>( SystemSetting.CloseOnEscape ) )
				{
					Hide();
				}
			}
			else if( args.Key == Key.Return )
			{
				if( CmdManager.CurrentItem != null )
				{
					ExecuteExecuteCommand();
					args.Handled = true;
				}
			}
			else if( args.Key == Key.Tab )
			{
				if( CmdManager.CurrentItem == null )
				{
					return;
				}

				args.Handled = true;

				if( !CommandInput.EndsWith( CmdManager.Separator ) )
				{
					_CommandInput += CmdManager.Separator;
					MessengerInstance.Send<InputCaretPositionMessage>( new InputCaretPositionMessage( CommandInput.Length ) );
				}

				List<string> commandParts = new List<string>( CmdManager.GetCommandParts( CommandInput ) );
				if( CommandInput.EndsWith( CmdManager.Separator ) )
				{
					commandParts.RemoveAt( 0 );
				}
				commandParts.RemoveAt( commandParts.Count - 1 );
				commandParts.Add( CmdManager.CurrentItem.Name );
				commandParts.Add( string.Empty );

				if( CmdManager.CurrentItem.Plugin.GetSubCommands( CmdManager.CurrentItem, commandParts ).Count() == 0 )
				{
					commandParts.RemoveAt( commandParts.Count - 1 );
				}
				else
				{
					CommandInput = string.Join( CmdManager.Separator, commandParts );
					MessengerInstance.Send<InputCaretPositionMessage>( new InputCaretPositionMessage( CommandInput.Length ) );
				}
			}
			else if( args.Key == Key.Up )
			{
				int idx = CmdManager.Items.IndexOf( CmdManager.CurrentItem );
				SelectedCommandIndex = Math.Max( 0, idx - 1 );
				args.Handled = true;
			}
			else if( args.Key == Key.Down )
			{
				int idx = CmdManager.Items.IndexOf( CmdManager.CurrentItem );
				SelectedCommandIndex = Math.Min( CmdManager.Items.Count, idx + 1 );
				args.Handled = true;
			}
			else if( args.Key == Key.Back )
			{
				if( CommandInput.EndsWith( CmdManager.Separator ) )
				{
					CommandInput = CommandInput.Substring( 0, CommandInput.Length - CmdManager.Separator.Length );
					MessengerInstance.Send<InputCaretPositionMessage>( new InputCaretPositionMessage( CommandInput.Length ) );

					args.Handled = true;
				}
			}
		}

		private void ExecuteSettingsCommand()
		{
			DialogServiceManager.Show<SettingsService>( Settings );
		}

		#endregion Commands

		#region Properties

		private string _CommandInfo;

		private string _CommandInput;

		private int _SelectedCommandIndex;

		public Blitzy.Model.CommandManager CmdManager { get; private set; }

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

		internal Database Database { get; private set; }

		internal PluginManager Plugins { get; private set; }

		internal Settings Settings { get; private set; }

		#endregion Properties

		#region Attributes

		internal HashSet<int> TaskList = new HashSet<int>();
		private object TaskListLock = new object();

		#endregion Attributes

		#region IPluginHost

		ISettings IPluginHost.Settings
		{
			get { return Settings; }
		}

		bool IPluginHost.IsPluginLoaded( System.Guid id )
		{
			return Plugins.IsLoaded( id );
		}

		#endregion IPluginHost
	}
}
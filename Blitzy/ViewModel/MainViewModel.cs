using System;
using System.Collections.ObjectModel;
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

		#endregion Constructor

		#region Methods

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

			MessengerInstance.Send<CommandMessage>( new CommandMessage( CommandStatus.Executing ) );
			Task.Run( () =>
			{
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
						MessengerInstance.Send<CommandMessage>( new CommandMessage( result ? CommandStatus.Finished : CommandStatus.Error, msg ) );
					} );
				}
			} );

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
				// FIXME: Hide window instead of closing it
				Close();
			}
		}

		private void ExecuteKeyPreviewCommand( KeyEventArgs args )
		{
			if( args.Key == Key.Escape )
			{
				if( Settings.GetValue<bool>( SystemSetting.CloseOnEscape ) )
				{
					// FIXME: Hide the window instead of closing it
					Close();
				}
			}
			else if( args.Key == Key.Return )
			{
				if( CmdManager.CurrentItem != null )
				{
					ExecuteExecuteCommand();
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

		internal Database Database { get; private set; }

		internal PluginManager Plugins { get; private set; }

		internal Settings Settings { get; private set; }

		#endregion Properties

		#region Attributes

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
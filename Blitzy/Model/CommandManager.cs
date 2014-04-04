﻿// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	public class CommandManager : LogObject
	{
		#region Constructor

		internal CommandManager( SQLiteConnection connection, Settings settings, PluginManager plugins )
		{
			Connection = connection;
			Settings = settings;
			Plugins = plugins;

			Items = new ObservableCollection<CommandItem>();
			Separator = " " + char.ConvertFromUtf32( 0x00002192 ) + " ";

			LoadPluginCommands();
		}

		#endregion Constructor

		#region Methods

		public void Clear( bool resetItem = true )
		{
			Items.Clear();
			if( resetItem )
			{
				CurrentItem = null;
			}
		}

		public string[] GetCommandParts( string text )
		{
			if( text.Trim().EndsWith( Separator ) )
			{
				return text.Split( new[] { Separator }, StringSplitOptions.None ).Concat( new[] { string.Empty } ).ToArray();
			}
			else
			{
				return text.Split( new[] { Separator }, StringSplitOptions.None );
			}
		}

		public void SearchItems( string text )
		{
			string[] parts = GetCommandParts( text );
			string command = parts[0].ToLowerInvariant();

			List<CommandItem> items = new List<CommandItem>();

			if( parts.Length == 1 )
			{
				items.AddRange( AvailableCommands.Where( cmd => cmd.CmdNames.Any( name => name.Match( text ) ) ) );
			}
			else if( CurrentItem != null )
			{
				items.AddRange( CurrentItem.Plugin.GetCommands( new Collection<string>( parts ) ) );
			}

			foreach( CommandItem item in items.OrderByDescending( it => GetCommandExecutionCount( it ) )
				.ThenByDescending( it => it.Name.GetDiceCoefficent( command ) ).Take( Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) ) )
			{
				Items.Add( item );
			}
		}

		public void UpdateExecutionCount( string itemName, Guid pluginId )
		{
			// Check if command was executed before
			bool registered = false;
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT ExecutionCount FROM commands WHERE Plugin = @plugin AND Name = @name;";
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "name";
				param.Value = itemName;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "plugin";
				param.Value = pluginId;
				cmd.Parameters.Add( param );

				registered = cmd.ExecuteScalar() != null;
			}

			// Update execution count of the command
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "name";
				param.Value = itemName;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "plugin";
				param.Value = pluginId;
				cmd.Parameters.Add( param );

				if( registered )
				{
					cmd.CommandText = "UPDATE commands SET ExecutionCount = ExecutionCount + 1 WHERE Plugin = @plugin AND Name = @name;";
				}
				else
				{
					cmd.CommandText = "INSERT INTO Blitzy_Commands (Plugin, Name, ExecutionCount) VALUES (@plugin, @name, 1);";
				}

				cmd.ExecuteNonQuery();
			}
		}

		private int GetCommandExecutionCount( CommandItem item )
		{
			int hash = 17;
			hash = hash * 23 + item.Plugin.PluginID.GetHashCode();
			hash = hash * 23 + item.Name.GetHashCode();

			if( !CommandExecutionBuffer.ContainsKey( hash ) )
			{
				CommandExecutionBuffer.Add( hash, 0 );
				return 0;
			}

			return CommandExecutionBuffer[hash];
		}

		private void LoadPluginCommands()
		{
			AvailableCommands.Clear();

			foreach( IPlugin plugin in Plugins.Plugins )
			{
				try
				{
					AvailableCommands.AddRange( plugin.GetCommands( new Collection<string>() ) );
				}
				catch( Exception ex )
				{
					LogError( "Failed to load commands from plugin {0}: {1}", plugin.Name, ex );
				}
			}
		}

		#endregion Methods

		#region Properties

		private CommandItem _CurrentItem;

		public CommandItem CurrentItem
		{
			get
			{
				return _CurrentItem;
			}

			set
			{
				if( _CurrentItem == value )
				{
					return;
				}

				RaisePropertyChanging( () => CurrentItem );
				_CurrentItem = value;
				RaisePropertyChanged( () => CurrentItem );
			}
		}

		public ObservableCollection<CommandItem> Items { get; private set; }

		public string Separator { get; private set; }

		#endregion Properties

		#region Attributes

		private List<CommandItem> AvailableCommands = new List<CommandItem>();
		private Dictionary<int, int> CommandExecutionBuffer = new Dictionary<int, int>();
		private SQLiteConnection Connection;
		private PluginManager Plugins;
		private Settings Settings;

		#endregion Attributes
	}
}
﻿// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using Blitzy.Plugin;
using Blitzy.Utility;

namespace Blitzy.Model
{
	public class CommandManager : BaseObject
	{
		#region Constructor

		internal CommandManager( SQLiteConnection connection, Settings settings, PluginManager plugins )
		{
			Connection = connection;
			Settings = settings;
			Plugins = plugins;

			AvailableCommands = new List<CommandItem>();
			Items = new ObservableCollection<CommandItem>();
			Separator = " " + char.ConvertFromUtf32( 0x00002192 ) + " ";

			LoadPluginCommands();
		}

		#endregion Constructor

		#region Methods

		public void Clear( bool resetItem = true )
		{
			CommandItem oldItem = CurrentItem;
			Items.Clear();
			CurrentItem = resetItem ? null : oldItem;
		}

		public string[] GetCommandParts( string text )
		{
			return text.Trim().EndsWith( Separator ) ?
				text.Split( new[] { Separator }, StringSplitOptions.None ).Concat( new[] { string.Empty } ).ToArray() :
				text.Split( new[] { Separator }, StringSplitOptions.None );
		}

		public void LoadPluginCommands()
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

		public void ResetExecutionCount()
		{
			CommandExecutionBuffer.Clear();

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM commands";
				cmd.ExecuteNonQuery();
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
				Collection<string> cmdParts = new Collection<string>( parts );

				IEnumerable<CommandItem> subCommands = CurrentItem.Plugin.GetSubCommands( CurrentItem, cmdParts ).ToArray();
				if( !subCommands.Any() )
				{
					items.Add( CurrentItem );
				}
				else
				{
					items.AddRange( subCommands );
				}
			}

			List<CommandItem> orderedItems = new List<CommandItem>( items.OrderByDescending( GetCommandExecutionCount ).Take( Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) ) );
			foreach( CommandItem item in orderedItems.OrderByDescending( GetCommandExecutionCount )
				.ThenByDescending( it => it.Name.GetDiceCoefficent( command ) ) )
			{
				Items.Add( item );
			}
		}

		public void UpdateExecutionCount( CommandItem item )
		{
			string itemName = item.Name;
			Guid pluginId = item.Plugin.PluginID;

			// Check if command was executed before
			bool registered;
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

				cmd.CommandText = registered ?
					"UPDATE commands SET ExecutionCount = ExecutionCount + 1 WHERE Plugin = @plugin AND Name = @name;" :
					"INSERT INTO commands (Plugin, Name, ExecutionCount) VALUES (@plugin, @name, 1);";

				cmd.ExecuteNonQuery();
			}

			int hash = item.GetHashCode();
			if( CommandExecutionBuffer.ContainsKey( hash ) )
			{
				CommandExecutionBuffer[hash]++;
			}
		}

		internal int GetCommandExecutionCount( CommandItem item )
		{
			int hash = item.GetHashCode();

			if( !CommandExecutionBuffer.ContainsKey( hash ) )
			{
				int count = ReadExecutionCount( item );
				CommandExecutionBuffer.Add( hash, count );
				return count;
			}

			return CommandExecutionBuffer[hash];
		}

		private int ReadExecutionCount( CommandItem item )
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "PluginID";
				param.Value = item.Plugin.PluginID;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Name";
				param.Value = item.Name;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT ExecutionCount FROM commands WHERE Plugin = @PluginID AND Name = @Name";

				return Convert.ToInt32( cmd.ExecuteScalar() );
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

		internal List<CommandItem> AvailableCommands { get; private set; }

		#endregion Properties

		#region Attributes

		private readonly Dictionary<int, int> CommandExecutionBuffer = new Dictionary<int, int>();
		private readonly SQLiteConnection Connection;
		private readonly PluginManager Plugins;
		private readonly Settings Settings;

		#endregion Attributes
	}
}
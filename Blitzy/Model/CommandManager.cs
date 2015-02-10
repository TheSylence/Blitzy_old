using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Linq;
using Blitzy.Plugin;
using Blitzy.Utility;

namespace Blitzy.Model
{
	public class CommandManager : BaseObject
	{
		internal CommandManager( DbConnectionFactory factory, Settings settings, PluginManager plugins )
		{
			ConnectionFactory = factory;
			Settings = settings;
			Plugins = plugins;

			AvailableCommands = new List<CommandItem>();
			Items = new ObservableCollection<CommandItem>();
			Separator = " " + char.ConvertFromUtf32( 0x00002192 ) + " ";

			LoadPluginCommands();
		}

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

			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "DELETE FROM commands";
					cmd.ExecuteNonQuery();
				}
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
			bool registered; using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT ExecutionCount FROM commands WHERE Plugin = @plugin AND Name = @name;";
					cmd.AddParameter( "name", itemName );
					cmd.AddParameter( "plugin", pluginId );

					registered = cmd.ExecuteScalar() != null;
				}

				// Update execution count of the command
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "name", itemName );
					cmd.AddParameter( "plugin", pluginId );

					cmd.CommandText = registered ?
						"UPDATE commands SET ExecutionCount = ExecutionCount + 1 WHERE Plugin = @plugin AND Name = @name;" :
						"INSERT INTO commands (Plugin, Name, ExecutionCount) VALUES (@plugin, @name, 1);";

					cmd.ExecuteNonQuery();
				}
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
			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "PluginID", item.Plugin.PluginID );
					cmd.AddParameter( "Name", item.Name );

					cmd.CommandText = "SELECT ExecutionCount FROM commands WHERE Plugin = @PluginID AND Name = @Name";

					return Convert.ToInt32( cmd.ExecuteScalar() );
				}
			}
		}

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

		private readonly Dictionary<int, int> CommandExecutionBuffer = new Dictionary<int, int>();
		private readonly PluginManager Plugins;
		private readonly Settings Settings;
		private CommandItem _CurrentItem;
		private DbConnectionFactory ConnectionFactory;
	}
}
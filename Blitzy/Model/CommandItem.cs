using System;
using System.Collections.Generic;
using System.Linq;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	public class CommandItem
	{
		private CommandItem()
		{
			CmdNames = new List<CommandName>();
		}

		#region Methods

		public static CommandItem Create( string name, string description, IPlugin plugin, string image = "",
			object userdata = null, CommandItem parent = null, IEnumerable<string> aliases = null, bool acceptsData = false )
		{
			if( string.IsNullOrWhiteSpace( name ) )
			{
				throw new ArgumentNullException( "name" );
			}

			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}

			CommandItem item = new CommandItem
			{
				Name = name,
				Description = description,
				Plugin = plugin,
				Icon = image,
				UserData = userdata,
				Parent = parent
			};

			item.CmdNames.Add( new CommandName( name ) );
			if( aliases != null )
			{
				item.CmdNames.AddRange( aliases.Select( a => new CommandName( a ) ) );
			}

			item.AcceptsData = acceptsData;

			return item;
		}

		public override int GetHashCode()
		{
			if( HashCode == 0 )
			{
				int hash = 17;
				hash = hash * 23 + Plugin.PluginID.GetHashCode();
				hash = hash * 23 + Name.GetHashCode();
				HashCode = hash;
			}

			return HashCode;
		}

		#endregion Methods

		#region Properties

		public bool AcceptsData { get; set; }

		public string Description { get; set; }

		public string Icon { get; set; }

		public string Name { get; internal set; }

		public CommandItem Parent { get; set; }

		public IPlugin Plugin { get; private set; }

		public object UserData { get; set; }

		internal List<CommandName> CmdNames { get; private set; }

		#endregion Properties

		#region Attributes

		private int HashCode;

		#endregion Attributes
	}
}
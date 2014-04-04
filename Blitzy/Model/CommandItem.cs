// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	public class CommandItem
	{
		#region Constructor

		private CommandItem()
		{
			CmdNames = new List<CommandName>();
		}

		#endregion Constructor

		#region Methods

		public static CommandItem Create( string name, string description, IPlugin plugin, string image = "",
			object userdata = null, CommandItem parent = null, IEnumerable<string> aliases = null )
		{
			if( string.IsNullOrWhiteSpace( name ) )
			{
				throw new ArgumentNullException( "name" );
			}

			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}

			CommandItem item = new CommandItem()
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

			return item;
		}

		#endregion Methods

		#region Properties

		public string Description { get; set; }

		public string Icon { get; set; }

		public string Name { get; internal set; }

		public CommandItem Parent { get; set; }

		public IPlugin Plugin { get; private set; }

		public object UserData { get; set; }

		public bool Visible { get; private set; }

		internal List<CommandName> CmdNames { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Reflection;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.System
{
	internal class Shelly : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "shell", "ShellyDescription".Localize(), this, "Shelly.png", null, null, new[] { "ps" } );
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			throw new NotImplementedException();
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			throw new NotImplementedException();
		}

		public void Unload( PluginUnloadReason reason )
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Properties

		private Guid? Guid;

		public int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Provides access to the PowerShell from within Blitzy"; }
		}

		public string Name
		{
			get { return "Shelly"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( "E27AF08C-62D9-45BF-95E2-28B7EA91A713" );
				}

				return Guid.Value;
			}
		}

		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		#endregion Properties
	}
}
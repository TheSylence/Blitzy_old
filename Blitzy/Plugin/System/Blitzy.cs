// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Plugin.System
{
	internal class Blitzy : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			// Nothing to do
		}

		public bool ExecuteCommand( CommandItem command, Collection<string> input, out string message )
		{
			Messenger.Default.Send<InternalCommandMessage>( new InternalCommandMessage( command.Name ) );

			message = null;
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( Collection<string> input )
		{
			yield return CommandItem.Create( "quit", "Quit Blitzy", this, "Quit.png" );
			yield return CommandItem.Create( "reset", "Reset command execution count", this, "Reset.png" );
			yield return CommandItem.Create( "catalog", "Rebuild the command catalog", this, "Rebuild.png" );
		}

		public string GetInfo( Collection<string> data, CommandItem item )
		{
			throw new NotImplementedException();
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			// Nothing to do
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
			Debug.Assert( reason != PluginUnloadReason.Unload );
		}

		#endregion Methods

		#region Properties

		private Guid? GUID;

		public int ApiVersion
		{
			get { return Constants.APIVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Internal Blitzy commands"; }
		}

		public string Name
		{
			get { return "Blitzy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( "DE0D9FF3-D089-4FFF-AA26-65785D2761E8" );
				}

				return GUID.Value;
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
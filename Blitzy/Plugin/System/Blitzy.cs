﻿// $Id$

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
using Blitzy.Utility;
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

		public bool ExecuteCommand( CommandItem command, IList<string> input, out string message )
		{
			Messenger.Default.Send<InternalCommandMessage>( new InternalCommandMessage( command.Name ) );

			message = null;
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "quit", "Quit".Localize(), this, "Quit.png" );
			yield return CommandItem.Create( "reset", "ResetCommand".Localize(), this, "Reset.png" );
			yield return CommandItem.Create( "catalog", "RebuildCatalog".Localize(), this, "Rebuild.png" );
			yield return CommandItem.Create( "version", "VersionCheck".Localize(), this, "Version.png" );
			yield return CommandItem.Create( "history", "ResetHistory".Localize(), this, "History.png" );

#if DEBUG
			yield return CommandItem.Create( "test", "Test Command", this );
#endif
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			if( parent.Name.Equals( "test" ) )
			{
				yield return CommandItem.Create( "test2", "Nested test", this );
			}
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
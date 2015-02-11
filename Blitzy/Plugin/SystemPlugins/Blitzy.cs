using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Blitzy : ISystemPlugin
	{
		public void ClearCache()
		{
			// Nothing to do
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		protected void Dispose( bool disposing )
		{
			if( disposing )
			{
			}
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			Messenger.Default.Send( new InternalCommandMessage( command.Name ) );

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
			yield return CommandItem.Create( "exception", "Throw an exception", this );
#endif
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return null;
		}

		public System.Windows.Controls.Control GetSettingsUI()
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
			get { return "Internal Blitzy commands"; }
		}

		public bool HasSettings { get { return false; } }

		public string Name
		{
			get { return "Blitzy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( "DE0D9FF3-D089-4FFF-AA26-65785D2761E8" );
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
	}
}
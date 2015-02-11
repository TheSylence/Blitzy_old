using System;
using System.Collections.Generic;
using Blitzy.Plugin;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockPlugin : IPlugin
	{
		public void ClearCache()
		{
			throw new NotImplementedException();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool ExecuteCommand( Blitzy.Model.CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetCommands( IList<string> input )
		{
			throw new NotImplementedException();
		}

		public string GetInfo( IList<string> data, Blitzy.Model.CommandItem item )
		{
			throw new NotImplementedException();
		}

		public IPluginViewModel GetSettingsDataContext( IViewServiceManager serviceManager )
		{
			return null;
		}

		public System.Windows.Controls.Control GetSettingsUI()
		{
			return null;
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetSubCommands( Blitzy.Model.CommandItem parent, IList<string> input )
		{
			yield return parent;
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			throw new NotImplementedException();
		}

		public void Unload( PluginUnloadReason reason )
		{
			throw new NotImplementedException();
		}

		public int ApiVersion
		{
			get { throw new NotImplementedException(); }
		}

		public string Author
		{
			get { throw new NotImplementedException(); }
		}

		public string Description
		{
			get { throw new NotImplementedException(); }
		}

		public bool HasSettings
		{
			get { return true; }
		}

		public string Name
		{
			get { return "MockPlugin"; }
		}

		public Guid PluginID
		{
			get
			{
				return Guid.Parse( "F3DA92EF-3E67-48B9-AC9D-06DCB94418CD" );
			}
		}

		public string Version
		{
			get { throw new NotImplementedException(); }
		}

		public Uri Website
		{
			get { throw new NotImplementedException(); }
		}
	}
}
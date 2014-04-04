// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;

namespace Blitzy.Tests.Mocks
{
	internal class MockPlugin : IPlugin
	{
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

		public string Name
		{
			get { throw new NotImplementedException(); }
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

		public void ClearCache()
		{
			throw new NotImplementedException();
		}

		public bool ExecuteCommand( Blitzy.Model.CommandItem command, System.Collections.ObjectModel.Collection<string> input, out string message )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetCommands( System.Collections.ObjectModel.Collection<string> input )
		{
			throw new NotImplementedException();
		}

		public string GetInfo( System.Collections.ObjectModel.Collection<string> data, Blitzy.Model.CommandItem item )
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
	}
}
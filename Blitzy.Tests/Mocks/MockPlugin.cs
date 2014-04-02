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
		public Guid PluginID
		{
			get
			{
				return Guid.Parse( "F3DA92EF-3E67-48B9-AC9D-06DCB94418CD" );
			}
		}

		public void ClearCache()
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
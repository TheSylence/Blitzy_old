// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;

namespace Blitzy.Tests.Mocks
{
	internal class MockPluginHost : IPluginHost
	{
		public ISettings Settings
		{
			get { throw new NotImplementedException(); }
		}

		public bool IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	public class Winy_Tests : TestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Winy() );
			Assert.IsTrue( tester.TestRunComplete );
		}
	}
}
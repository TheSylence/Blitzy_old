// $Id$

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Blitzy_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Blitzy.Plugin.System.Blitzy() );
			Assert.IsTrue( tester.TestRunComplete );
		}
	}
}
﻿using Blitzy.Plugin.SystemPlugins;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Calcy_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Calcy() );
			Assert.IsTrue( tester.TestRunComplete );
		}
	}
}
using System;
using Blitzy.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class PluginTester
	{
		public PluginTester( IPlugin plug )
		{
			Assert.IsNotNull( plug, "Plugin null" );

			Assert.IsNotNull( plug.Author, "Plugin Author" );
			Assert.IsNotNull( plug.Description, "Plugin Description" );
			Assert.IsNotNull( plug.Version, "Plugin Version" );
			Assert.AreEqual( Constants.ApiVersion, plug.ApiVersion, "Plugin APIVersion" );
			Assert.AreNotEqual( Guid.Empty, plug.PluginID, "Plugin ID" );
			Assert.IsNotNull( plug.Website, "Plugin Website" );
			Assert.IsNotNull( plug.Name, "Plugin Name" );

			TestRunComplete = true;
		}

		public readonly bool TestRunComplete;
	}
}


using Blitzy.Plugin;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class PluginManager_Tests : TestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void GetPluginTest()
		{
			MockPluginHost host = new MockPluginHost();
			using( PluginManager pmgr = new PluginManager( host, Connection ) )
			{
				pmgr.LoadPlugins();
				Assert.AreNotEqual( 0, pmgr.Plugins.Count );

				foreach( IPlugin plug in pmgr.Plugins )
				{
					Assert.IsTrue( pmgr.IsLoaded( plug.PluginID ) );

					Assert.AreSame( plug, pmgr.GetPlugin( plug.PluginID ) );
					Assert.AreSame( plug, pmgr.GetPlugin( plug.Name ) );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void IsLoadedTest()
		{
			MockPluginHost host = new MockPluginHost();
			using( PluginManager pmgr = new PluginManager( host, Connection ) )
			{
				pmgr.LoadPlugins();

				Blitzy.Plugin.SystemPlugins.Blitzy plug = new Plugin.SystemPlugins.Blitzy();

				Assert.IsTrue( pmgr.IsLoaded( plug.PluginID ) );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void LoadTest()
		{
			MockPluginHost host = new MockPluginHost();
			using( PluginManager pmgr = new PluginManager( host, Connection ) )
			{
				pmgr.LoadPlugins();

				Assert.AreNotEqual( 0, pmgr.Plugins.Count );
			}
		}
	}
}
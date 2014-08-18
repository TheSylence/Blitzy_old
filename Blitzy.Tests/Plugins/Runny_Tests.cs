// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin.SystemPlugins;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Runny_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void ExecuteTest()
		{
			Runny plug = new Runny();

			using( ShimsContext.Create() )
			{
				ProcessStartInfo receivedInfo = null;
				System.Diagnostics.Fakes.ShimProcess.StartProcessStartInfo = ( inf ) =>
				{
					receivedInfo = inf;
					return new System.Diagnostics.Fakes.StubProcess();
				};

				string msg = null;
				Assert.IsTrue( plug.ExecuteCommand( CommandItem.Create( "test", "test.exe", plug ), Plugin.CommandExecutionMode.Default, null, out msg ) );
				Assert.IsTrue( string.IsNullOrEmpty( msg ) );

				Assert.IsNotNull( receivedInfo );
				Assert.AreEqual( "test.exe", receivedInfo.FileName );

				////////////

				receivedInfo = null;
				Assert.IsTrue( plug.ExecuteCommand( CommandItem.Create( "test", "test.exe", plug ), Plugin.CommandExecutionMode.Secondary, null, out msg ) );
				Assert.IsTrue( string.IsNullOrEmpty( msg ) );

				Assert.IsNotNull( receivedInfo );
				Assert.AreEqual( "test.exe", receivedInfo.FileName );
				Assert.AreEqual( "runas", receivedInfo.Verb );

				////////////

				using( Workspace ws = new Workspace() )
				{
					using( WorkspaceItem item = new WorkspaceItem() )
					{
						item.ItemCommand = "command.exe";
						ws.Items.Add( item );

						receivedInfo = null;
						Assert.IsTrue( plug.ExecuteCommand( CommandItem.Create( "test", "test.exe", plug, "", ws ), Plugin.CommandExecutionMode.Default, null, out msg ) );
						Assert.IsTrue( string.IsNullOrEmpty( msg ) );

						Assert.IsNotNull( receivedInfo );
						Assert.AreEqual( "command.exe", receivedInfo.FileName );
					}
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Blitzy.Plugin.SystemPlugins.Runny() );
			Assert.IsTrue( tester.TestRunComplete );
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin.System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	public class Weby_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void ExecuteCommandTest()
		{
			Weby plug = new Weby();
			plug.Load( this );

			IEnumerable<CommandItem> commands = plug.GetCommands( new[] { "weby" } );
			Assert.AreEqual( 1, commands.Count() );

			CommandItem cmd = commands.FirstOrDefault();
			Assert.IsNotNull( cmd );
			Assert.AreEqual( "weby", cmd.Name );

			string message;
			bool result;
			string url = null;

			using( ShimsContext.Create() )
			{
				System.Diagnostics.Fakes.ShimProcess.StartString = s =>
				{
					url = s;
					return new System.Diagnostics.Fakes.StubProcess();
				};

				result = plug.ExecuteCommand( cmd, new[] { "weby", "example.invalid" }, out message );
			}

			Assert.IsTrue( result );
			Assert.AreEqual( "http://example.invalid/", url );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Weby() );
			Assert.IsTrue( tester.TestRunComplete );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void LoadTest()
		{
			Weby plug = new Weby();
			Assert.IsTrue( plug.Load( this ) );

			// TODO: Check if websites are saved in the database
		}
	}
}
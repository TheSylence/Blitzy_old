using System.Collections.Generic;
using System.Linq;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Plugin.SystemPlugins;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Putty_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void ExecuteCommandTest()
		{
			Putty plug = new Putty();
			( (IPluginHost)this ).Settings.SetValue( plug, Putty.PathKey, "putty.exe" );
			Assert.IsTrue( plug.Load( this ) );

			CommandItem command = plug.GetCommands( new List<string>() ).FirstOrDefault();
			Assert.IsNotNull( command );

			bool result;
			string proc = null;
			string args = null;
			using( ShimsContext.Create() )
			{
				System.Diagnostics.Fakes.ShimProcess.StartStringString = ( s, a ) =>
				{
					proc = s;
					args = a;
					return new System.Diagnostics.Fakes.StubProcess();
				};

				string message;
				result = plug.ExecuteCommand( command, CommandExecutionMode.Default, new[] { "test@localhost" }, out message );
			}

			Assert.IsTrue( result );
			Assert.AreEqual( "putty.exe", proc );
			Assert.AreEqual( "test@localhost", args );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Putty() );
			Assert.IsTrue( tester.TestRunComplete );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void LoadTest()
		{
			Putty plug = new Putty();
			Assert.IsTrue( plug.Load( this ) );

			IEnumerable<CommandItem> commands = plug.GetCommands( new List<string>() );
			Assert.AreEqual( 1, commands.Count() );
		}
	}
}
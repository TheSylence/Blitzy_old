// $Id$

using System.Collections.Generic;
using System.Linq;
using Blitzy.Model;
using Blitzy.Plugin.System;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Weby_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void ExecuteCommandTest()
		{
			Weby plug = new Weby();
			Assert.IsTrue( plug.Load( this ) );

			Dictionary<string, string> expectedUrls = new Dictionary<string, string>();
			expectedUrls.Add( "google", "google.com" );
			expectedUrls.Add( "wiki", "wikipedia.org" );
			expectedUrls.Add( "youtube", "youtube.com" );
			expectedUrls.Add( "bing", "bing.com" );
			expectedUrls.Add( "facebook", "facebook.com" );
			expectedUrls.Add( "wolfram", "wolframalpha.com" );
			expectedUrls.Add( "weby", "http://test" );

			IEnumerable<CommandItem> commands = plug.GetCommands( new List<string>() );
			Assert.AreEqual( expectedUrls.Count, commands.Count() );

			string message;
			bool result;
			string url = null;

			int startedProcesses = 0;
			foreach( CommandItem command in commands )
			{
				using( ShimsContext.Create() )
				{
					System.Diagnostics.Fakes.ShimProcess.StartString = s =>
					{
						++startedProcesses;
						url = s;
						return new System.Diagnostics.Fakes.StubProcess();
					};

					result = plug.ExecuteCommand( command, Plugin.CommandExecutionMode.Default, new[] { command.Name, "test" }, out message );
				}

				Assert.IsTrue( result );
				Assert.IsTrue( url.Contains( "test" ) );
				Assert.IsTrue( url.Contains( expectedUrls[command.Name] ) );
			}

			Assert.AreEqual( commands.Count(), startedProcesses );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Weby() );
			Assert.IsTrue( tester.TestRunComplete );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void IsUrlTest()
		{
			Assert.IsTrue( Weby.IsUrl( "http://google.com" ) );
			Assert.IsTrue( Weby.IsUrl( "ftp://user:pass@domain.com" ) );
			Assert.IsTrue( Weby.IsUrl( "file://local/file.txt" ) );
			Assert.IsTrue( Weby.IsUrl( "https://secure.website.com" ) );

			Assert.IsFalse( Weby.IsUrl( "htp://typo.com" ) );
			Assert.IsFalse( Weby.IsUrl( "steam://steamapp" ) );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void LoadTest()
		{
			Weby plug = new Weby();
			Assert.IsTrue( plug.Load( this ) );

			IEnumerable<CommandItem> commands = plug.GetCommands( new List<string>() );
			Assert.AreEqual( 7, commands.Count() );
		}
	}
}
﻿// $Id$

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

			foreach( CommandItem command in commands )
			{
				if( command.Name == "weby" )
				{
					continue;
				}

				using( ShimsContext.Create() )
				{
					System.Diagnostics.Fakes.ShimProcess.StartString = s =>
					{
						url = s;
						return new System.Diagnostics.Fakes.StubProcess();
					};

					result = plug.ExecuteCommand( command, new[] { command.Name, "test" }, out message );
				}

				Assert.IsTrue( result );
				Assert.IsTrue( url.Contains( "test" ) );
				Assert.IsTrue( url.Contains( expectedUrls[command.Name] ) );
			}
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

			IEnumerable<CommandItem> commands = plug.GetCommands( new List<string>() );
			Assert.AreEqual( 7, commands.Count() );
		}
	}
}
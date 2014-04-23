// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CommandName_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void FindCamelCasingTest()
		{
			CommandName cmd = new CommandName( "Visual Studio Command Prompt" );

			Assert.IsTrue( cmd.Match( "vscp" ) );
			Assert.IsTrue( cmd.Match( "vsc" ) );
			Assert.IsFalse( cmd.Match( "vspc" ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void FindContainTest()
		{
			CommandName cmd = new CommandName( "This is a test" );

			Assert.IsTrue( cmd.Match( "is" ) );
			Assert.IsTrue( cmd.Match( "this" ) );
			Assert.IsFalse( cmd.Match( "foo" ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void FindShuffleWordsTest()
		{
			CommandName cmd = new CommandName( "Metallica - Nothing else matters" );

			Assert.IsTrue( cmd.Match( "nothing else metallica" ) );
			Assert.IsFalse( cmd.Match( "nothing else test" ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void FindWithoutSymbolsTest()
		{
			CommandName cmd = new CommandName( "Metallica - Nothing else matters" );

			Assert.IsTrue( cmd.Match( "metallica nothing" ) );
			Assert.IsTrue( cmd.Match( "metallica nothing else matters test" ) );
			Assert.IsFalse( cmd.Match( "test" ) );
			Assert.IsFalse( cmd.Match( "metallica else test" ) );
		}
	}
}
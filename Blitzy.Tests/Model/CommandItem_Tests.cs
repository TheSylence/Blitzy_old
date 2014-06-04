// $Id$

using System;
using System.Linq;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CommandItem_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void CreateTest()
		{
			MockPlugin plugin = new MockPlugin();
			CommandItem item = CommandItem.Create( "name", "description", plugin, "image", 123, null, new[] { "alias1", "alias2" } );

			Assert.IsNotNull( item );
			Assert.AreEqual( "name", item.Name );
			Assert.AreEqual( "description", item.Description );
			Assert.AreSame( plugin, item.Plugin );
			Assert.AreEqual( "image", item.Icon );
			Assert.AreEqual( 123, item.UserData );

			Assert.AreEqual( 3, item.CmdNames.Count );
			Assert.AreEqual( 1, item.CmdNames.Where( c => c.OrigName.Equals( "name" ) ).Count() );
			Assert.AreEqual( 1, item.CmdNames.Where( c => c.OrigName.Equals( "alias1" ) ).Count() );
			Assert.AreEqual( 1, item.CmdNames.Where( c => c.OrigName.Equals( "alias2" ) ).Count() );
		}

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void NameNullTest()
		{
			MockPlugin plugin = new MockPlugin();
			CommandItem.Create( null, null, plugin );
		}

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void PluginNullTest()
		{
			CommandItem.Create( "Test", null, null );
		}
	}
}
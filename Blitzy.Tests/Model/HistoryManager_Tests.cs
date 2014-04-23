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
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class HistoryManager_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void ClearTest()
		{
			Settings cfg = new Settings( Connection );

			HistoryManager mgr = new HistoryManager( cfg );
			mgr.AddItem( "item1" );
			mgr.AddItem( "item2" );
			mgr.AddItem( "item3" );

			mgr.Clear();
			mgr.Save();

			mgr = new HistoryManager( cfg );
			Assert.AreEqual( 0, mgr.Commands.Count );
		}

		[TestMethod, TestCategory( "Model" )]
		public void DoubleAddTest()
		{
			Settings cfg = new Settings( Connection );
			HistoryManager mgr = new HistoryManager( cfg );

			mgr.AddItem( "item" );
			mgr.AddItem( "item" );

			Assert.AreEqual( 1, mgr.Commands.Count );
		}

		[TestMethod, TestCategory( "Model" )]
		public void OverflowTest()
		{
			Settings cfg = new Settings( Connection );
			cfg.SetValue( SystemSetting.HistoryCount, 3 );
			HistoryManager mgr = new HistoryManager( cfg );

			mgr.AddItem( "item1" );
			mgr.AddItem( "item2" );
			mgr.AddItem( "item3" );
			mgr.AddItem( "item4" );

			Assert.IsFalse( mgr.Commands.Contains( "item1" ) );
			Assert.IsTrue( mgr.Commands.Contains( "item4" ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			Settings cfg = new Settings( Connection );

			HistoryManager mgr = new HistoryManager( cfg );
			mgr.AddItem( "item1" );
			mgr.AddItem( "item2" );
			mgr.AddItem( "item3" );

			mgr.Save();

			mgr = new HistoryManager( cfg );
			Assert.AreEqual( 3, mgr.Commands.Count );
			Assert.IsTrue( mgr.Commands.Contains( "item1" ) );
			Assert.IsTrue( mgr.Commands.Contains( "item2" ) );
			Assert.IsTrue( mgr.Commands.Contains( "item3" ) );
		}
	}
}
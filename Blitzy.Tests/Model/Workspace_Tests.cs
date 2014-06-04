// $Id$

using System;
using System.Linq;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Workspace_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void DeleteTest()
		{
			Workspace w = new Workspace();
			w.ID = 1;
			w.Name = "test";
			w.Save( Connection );

			w = new Workspace();
			w.ID = 1;
			w.Delete( Connection );

			w = new Workspace();
			w.ID = 1;
			w.Load( Connection );
		}

		[TestMethod, TestCategory( "Model" )]
		public void ItemsTest()
		{
			WorkspaceItem item = new WorkspaceItem();
			item.ItemID = 2;
			item.WorkspaceID = 1;
			item.ItemCommand = "test";

			item.Save( Connection );

			item = new WorkspaceItem();
			item.ItemID = 1;
			item.WorkspaceID = 1;
			item.ItemCommand = "test";

			item.Save( Connection );

			Workspace w = new Workspace();
			w.ID = 1;
			w.Name = "test";
			w.Save( Connection );

			w = new Workspace();
			w.ID = 1;
			w.Load( Connection );

			Assert.AreEqual( 2, w.Items.Count );
			Assert.AreEqual( 1, w.Items.Count( i => i.ItemID == 1 ) );
			Assert.AreEqual( 1, w.Items.Count( i => i.ItemID == 2 ) );
		}

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void LoadNonExistingTest()
		{
			Workspace w = new Workspace();
			w.ID = int.MaxValue;

			w.Load( Connection );
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			Workspace obj = new Workspace();
			PropertyChangedListener listener = new PropertyChangedListener( obj );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			Workspace w = new Workspace();
			Assert.IsFalse( w.ExistsInDatabase );

			w.Name = "google";
			w.ID = 1;

			w.Items.Add( new WorkspaceItem() { ItemID = 1, ItemCommand = "test", WorkspaceID = 1 } );
			w.Items.Add( new WorkspaceItem() { ItemID = 2, ItemCommand = "test2", WorkspaceID = 1 } );

			w.Save( Connection );

			Assert.IsTrue( w.ExistsInDatabase );

			Workspace w2 = new Workspace();
			w2.ID = 1;
			w2.Load( Connection );

			Assert.IsTrue( w2.ExistsInDatabase );
			Assert.AreEqual( w.Name, w2.Name );
			Assert.AreEqual( 2, w2.Items.Count );
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			Workspace w = new Workspace();
			Assert.IsFalse( w.ExistsInDatabase );

			w.Name = "ws1";
			w.ID = 1;

			w.Save( Connection );
			Assert.IsTrue( w.ExistsInDatabase );

			w.Name = "helloworld";
			w.Save( Connection );

			w = new Workspace();
			w.ID = 1;
			w.Load( Connection );

			Assert.AreEqual( "helloworld", w.Name );
		}
	}
}
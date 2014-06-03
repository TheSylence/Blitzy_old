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
	public class WorkspaceItem_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void LoadNonExistingTest()
		{
			WorkspaceItem w = new WorkspaceItem();
			w.ItemID = int.MaxValue;

			w.Load( Connection );
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			WorkspaceItem obj = new WorkspaceItem();
			PropertyChangedListener listener = new PropertyChangedListener( obj );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			WorkspaceItem w = new WorkspaceItem();
			Assert.IsFalse( w.ExistsInDatabase );

			w.ItemCommand = "google";
			w.ItemID = 1;

			w.Save( Connection );

			Assert.IsTrue( w.ExistsInDatabase );

			WorkspaceItem w2 = new WorkspaceItem();
			w2.ItemID = 1;
			w2.Load( Connection );

			Assert.IsTrue( w2.ExistsInDatabase );
			Assert.AreEqual( w.ItemCommand, w2.ItemCommand );
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			WorkspaceItem w = new WorkspaceItem();
			Assert.IsFalse( w.ExistsInDatabase );

			w.ItemCommand = "ws1";
			w.ItemID = 1;

			w.Save( Connection );
			Assert.IsTrue( w.ExistsInDatabase );

			w.ItemCommand = "helloworld";
			w.Save( Connection );

			w = new WorkspaceItem();
			w.ItemID = 1;
			w.Load( Connection );

			Assert.AreEqual( "helloworld", w.ItemCommand );
		}
	}
}
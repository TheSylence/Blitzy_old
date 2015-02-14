using System;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WorkspaceItem_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void DeleteTest()
		{
			int id = TestHelper.NextID();

			using( WorkspaceItem w = new WorkspaceItem() )
			{
				w.ItemID = id;
				w.ItemCommand = "test";
				w.Save( Connection );
			}
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				w.ItemID = id;
					w.Load( Connection );

				w.Delete( Connection );
			}
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				w.ItemID = id;
				ExceptionAssert.Throws<TypeLoadException>( () => w.Load( Connection ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( w );

				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			int id = TestHelper.NextID();
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				Assert.IsFalse( w.ExistsInDatabase );

				w.ItemCommand = "google";
				w.ItemID = id;

				w.Save( Connection );

				Assert.IsTrue( w.ExistsInDatabase );

				using( WorkspaceItem w2 = new WorkspaceItem() )
				{
				w2.ItemID = id;
				w2.Load( Connection );

				Assert.IsTrue( w2.ExistsInDatabase );
				Assert.AreEqual( w.ItemCommand, w2.ItemCommand );
			}
			}

			using( WorkspaceItem w = new WorkspaceItem() )
			{
				w.ItemID = int.MaxValue;

				ExceptionAssert.Throws<TypeLoadException>( () => w.Load( Connection ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			int id = TestHelper.NextID();
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				Assert.IsFalse( w.ExistsInDatabase );

				w.ItemCommand = "ws1";
				w.ItemID = id;

				w.Save( Connection );
				Assert.IsTrue( w.ExistsInDatabase );

				w.ItemCommand = "helloworld";
				w.Save( Connection );
			}
			using( WorkspaceItem w = new WorkspaceItem() )
			{
				w.ItemID = id;
				w.Load( Connection );

				Assert.AreEqual( "helloworld", w.ItemCommand );
			}
		}
	}
}
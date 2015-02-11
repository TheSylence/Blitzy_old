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
		[TestMethod, TestCategory( "Model" )]
		public void DeleteTest()
		{
			int id = TestHelper.NextID();

			using( Workspace w = new Workspace() )
			{
				w.ID = id;
				w.Name = "test";
				w.Save( Connection );
			}

			using( Workspace w = new Workspace() )
			{
				w.ID = id;
				w.Delete( Connection );
			}

			using( Workspace w = new Workspace() )
			{
				w.ID = id;
				ExceptionAssert.Throws<TypeLoadException>( () => w.Load( Connection ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void ItemsTest()
		{
			int id1 = TestHelper.NextID();
			int id2 = TestHelper.NextID();
			int wsid = TestHelper.NextID();

			using( WorkspaceItem item = new WorkspaceItem() )
			{
				item.ItemID = id1;
				item.WorkspaceID = wsid;
				item.ItemCommand = "test";

				item.Save( Connection );
			}

			using( WorkspaceItem item = new WorkspaceItem() )
			{
				item.ItemID = id2;
				item.WorkspaceID = wsid;
				item.ItemCommand = "test";

				item.Save( Connection );
			}

			using( Workspace w = new Workspace() )
			{
				w.ID = wsid;
				w.Name = "test";
				w.Save( Connection );
			}
			using( Workspace w = new Workspace() )
			{
				w.ID = wsid;
				w.Load( Connection );

				Assert.AreEqual( 2, w.Items.Count );
				Assert.AreEqual( 1, w.Items.Count( i => i.ItemID == id1 ) );
				Assert.AreEqual( 1, w.Items.Count( i => i.ItemID == id2 ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			using( Workspace w = new Workspace() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( w );

				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			int id = TestHelper.NextID();
			using( Workspace w = new Workspace() )
			{
				Assert.IsFalse( w.ExistsInDatabase );

				w.Name = "google";
				w.ID = id;

				using( WorkspaceItem item1 = new WorkspaceItem() { ItemID = TestHelper.NextID(), ItemCommand = "test", WorkspaceID = id } )
				{
					w.Items.Add( item1 );
					using( WorkspaceItem item2 = new WorkspaceItem() { ItemID = TestHelper.NextID(), ItemCommand = "test2", WorkspaceID = id } )
					{
						w.Items.Add( item2 );

				w.Save( Connection );

				Assert.IsTrue( w.ExistsInDatabase );

						using( Workspace w2 = new Workspace() )
						{
				w2.ID = id;
				w2.Load( Connection );

				Assert.IsTrue( w2.ExistsInDatabase );
				Assert.AreEqual( w.Name, w2.Name );
				Assert.AreEqual( 2, w2.Items.Count );
			}
					}
				}
			}

			using( Workspace w = new Workspace() )
			{
				w.ID = int.MaxValue;

				ExceptionAssert.Throws<TypeLoadException>( () => w.Load( Connection ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			int id = TestHelper.NextID();
			using( Workspace w = new Workspace() )
			{
				Assert.IsFalse( w.ExistsInDatabase );

				w.Name = "ws1";
				w.ID = id;

				w.Save( Connection );
				Assert.IsTrue( w.ExistsInDatabase );

				w.Name = "helloworld";
				w.Save( Connection );
			}
			using( Workspace w = new Workspace() )
			{
				w.ID = id;
				w.Load( Connection );

				Assert.AreEqual( "helloworld", w.Name );
			}
		}
	}
}
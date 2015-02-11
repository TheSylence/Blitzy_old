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
			using( Settings cfg = new Settings( ConnectionFactory ) )
			{
				using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
				{
					mgr.AddItem( "item1" );
					mgr.AddItem( "item2" );
					mgr.AddItem( "item3" );

					mgr.Clear();
					mgr.Save();
				}

				using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
				{
					Assert.AreEqual( 0, mgr.Commands.Count );
				}
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void DoubleAddTest()
		{
			using( Settings cfg = new Settings( ConnectionFactory ) )
			using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
			{
				mgr.AddItem( "item" );
				mgr.AddItem( "item" );

				Assert.AreEqual( 1, mgr.Commands.Count );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void OverflowTest()
		{
			using( Settings cfg = new Settings( ConnectionFactory ) )
			{
				cfg.SetValue( SystemSetting.HistoryCount, 3 );
				using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
				{
					mgr.AddItem( "item1" );
					mgr.AddItem( "item2" );
					mgr.AddItem( "item3" );
					mgr.AddItem( "item4" );

					Assert.IsFalse( mgr.Commands.Contains( "item1" ) );
					Assert.IsTrue( mgr.Commands.Contains( "item4" ) );
				}
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			using( Settings cfg = new Settings( ConnectionFactory ) )
			{
				using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
				{
					mgr.AddItem( "item1" );
					mgr.AddItem( "item2" );
					mgr.AddItem( "item3" );

					mgr.Save();
				}

				using( HistoryManager mgr = new HistoryManager( ConnectionFactory, cfg ) )
				{
					Assert.AreEqual( 3, mgr.Commands.Count );
					Assert.IsTrue( mgr.Commands.Contains( "item1" ) );
					Assert.IsTrue( mgr.Commands.Contains( "item2" ) );
					Assert.IsTrue( mgr.Commands.Contains( "item3" ) );
				}
			}
		}
	}
}
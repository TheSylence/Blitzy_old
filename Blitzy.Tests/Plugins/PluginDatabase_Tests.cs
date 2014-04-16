// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	public class PluginDatabase_Tests : TestBase
	{
		private IPlugin Plugin { get; set; }

		[TestInitialize()]
		public override void BeforeTestRun()
		{
			base.BeforeTestRun();

			Plugin = new MockPlugin();
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void CreateAndDropTableTest()
		{
			PluginDatabase db = new PluginDatabase( Connection );

			TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

			Assert.IsTrue( db.CreateTable( Plugin, "test", columns ) );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'MockPlugin_test'";

				string result = (string)cmd.ExecuteScalar();
				Assert.AreEqual( "MockPlugin_test", result );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT PluginID FROM plugin_tables WHERE TableName = 'MockPlugin_test'";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					Assert.IsTrue( reader.Read() );

					Assert.AreEqual( Plugin.PluginID, reader.GetGuid( 0 ) );
				}
			}

			db.DropTable( Plugin, "test" );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'MockPlugin_test'";

				Assert.IsNull( cmd.ExecuteScalar() );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT PluginID FROM plugin_tables WHERE TableName = 'MockPlugin_test'";

				Assert.IsNull( cmd.ExecuteScalar() );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InsertUpdateDeleteTest()
		{
			PluginDatabase db = new PluginDatabase( Connection );

			TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

			db.CreateTable( Plugin, "datatest", columns );

			Dictionary<string, object> values = new Dictionary<string, object>();
			values.Add( "col1", 12 );
			values.Add( "col2", "test" );
			db.Insert( Plugin, "datatest", values );

			IEnumerable<IDictionary<string, object>> result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
			Assert.AreEqual( 1, result.Count() );

			IDictionary<string, object> row = result.First();
			Assert.AreEqual( "12", row["col1"].ToString() );
			Assert.AreEqual( "test", row["col2"] );

			db.Update( Plugin, "datatest", new Dictionary<string, object> { { "col1", 123 }, { "col2", "col" } } );

			result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
			Assert.AreEqual( 1, result.Count() );

			row = result.First();
			Assert.AreEqual( "123", row["col1"].ToString() );
			Assert.AreEqual( "col", row["col2"] );

			db.Delete( Plugin, "datatest" );

			result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
			Assert.AreEqual( 0, result.Count() );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void TransactionTest()
		{
			PluginDatabase db = new PluginDatabase( Connection );
			DbTransaction trans = db.BeginTransaction();

			TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

			db.CreateTable( Plugin, "test", columns );

			trans.Rollback();

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'MockPlugin_test'";

				Assert.IsNull( cmd.ExecuteScalar() );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT PluginID FROM plugin_tables WHERE TableName = 'MockPlugin_test'";

				Assert.IsNull( cmd.ExecuteScalar() );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void WhereTest()
		{
			PluginDatabase db = new PluginDatabase( Connection );
			DbTransaction trans = db.BeginTransaction();

			TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

			db.CreateTable( Plugin, "wheretest", columns );

			for( int i = 0; i < 10; ++i )
			{
				Dictionary<string, object> values = new Dictionary<string, object>();
				values.Add( "col1", i );
				values.Add( "col2", "test" + i.ToString() );
				db.Insert( Plugin, "wheretest", values );
			}

			WhereClause where = new WhereClause();
			where.AddCondition( "col1", 5, WhereOperation.Less );

			db.Update( Plugin, "wheretest", new Dictionary<string, object> { { "col2", "test" } }, where );

			IEnumerable<IDictionary<string, object>> result = db.Select( Plugin, "wheretest", new[] { "col2" }, where );
			Assert.AreEqual( 5, result.Count() );

			Assert.IsTrue( result.All( dict => dict.Values.All( v => v.Equals( "test" ) ) ) );

			db.Delete( Plugin, "wheretest", where );

			result = db.Select( Plugin, "wheretest", new[] { "col2" }, where );
			Assert.AreEqual( 0, result.Count() );
		}
	}
}
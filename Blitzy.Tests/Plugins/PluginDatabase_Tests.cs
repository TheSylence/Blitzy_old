// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

				Assert.IsFalse( db.CreateTable( null, "test", columns ) );

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
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionCreateTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				db.CreateTable( Plugin, "] (Temp INTEGER PRIMARY KEY); DROP TABLE settings ;-- ", new[] { new TableColumn() } );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}

				db.CreateTable( Plugin, "testtable", new[] { new TableColumn( "temp INTEGER PRIMARY KEY); DROP TABLE settings ;-- ", ColumnType.Text ) } );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionDeleteTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				db.Delete( Plugin, "]; DROP TABLE settings; -- " );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionDropTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				db.DropTable( Plugin, "]; DROP TABLE settings; -- " );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionInsertTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				Dictionary<string, object>[] values = new Dictionary<string, object>[]
			{
				new Dictionary<string,object>{ {"col1", 1}, {"col2", "1"} },
				new Dictionary<string,object>{ {"col1", 2}, {"col2", "2"} },
				new Dictionary<string,object>{ {"col1", 3}, {"col2", "3"} },
			};
				db.Insert( Plugin, "]; DROP TABLE settings; -- ", values );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}

				values = new Dictionary<string, object>[]
			{
				new Dictionary<string,object>{ {"col1) VALUES(1); DROP TABLE settings; -- ", 1}, {"col2", "1"} }
			};
				db.Insert( Plugin, "temptable", values );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}

				values = new Dictionary<string, object>[]
			{
				new Dictionary<string,object>{ {"col1", 1}, {"col2", "1); DROP TABLE settings; -- "} }
			};
				db.Insert( Plugin, "temptable", values );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionSelectTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				db.Select( Plugin, "]; DROP TABLE settings; -- ", new[] { "col1" } );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InjectionUpdatTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				db.Update( Plugin, "]; DROP TABLE settings; -- ", new Dictionary<string, object> { { "col1", 1 }, { "col2", "1" } } );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}

				db.Update( Plugin, "temptable", new Dictionary<string, object> { { "col1 = 1; DROP TABLE settings; -- ", 1 }, { "col2", "1" } } );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}

				db.Update( Plugin, "]; DROP TABLE settings; -- ", new Dictionary<string, object> { { "col1", 1 }, { "col2", "1; DROP TABLE settings; -- " } } );
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT name FROM sqlite_master WHERE type = 'table' AND name = 'settings'";

					string result = (string)cmd.ExecuteScalar();
					Assert.AreEqual( "settings", result );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InsertUpdateDeleteTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

				db.CreateTable( Plugin, "datatest", columns );

				Dictionary<string, object> values = new Dictionary<string, object>();
				values.Add( "col1", 12 );
				values.Add( "col2", "test" );
				Assert.AreEqual( 1, db.Insert( Plugin, "datatest", values ) );

				IEnumerable<IDictionary<string, object>> result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
				Assert.AreEqual( 1, result.Count() );

				IDictionary<string, object> row = result.First();
				Assert.AreEqual( "12", row["col1"].ToString() );
				Assert.AreEqual( "test", row["col2"] );

				Assert.AreEqual( 1, db.Update( Plugin, "datatest", new Dictionary<string, object> { { "col1", 123 }, { "col2", "col" } } ) );

				result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
				Assert.AreEqual( 1, result.Count() );

				row = result.First();
				Assert.AreEqual( "123", row["col1"].ToString() );
				Assert.AreEqual( "col", row["col2"] );

				db.Delete( Plugin, "datatest" );

				result = db.Select( Plugin, "datatest", new[] { "col1", "col2" } );
				Assert.AreEqual( 0, result.Count() );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void MayAccessTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				Assert.IsFalse( db.MayAccess( Plugin, string.Empty ) );
				Assert.IsFalse( db.MayAccess( Plugin, null ) );
				Assert.IsFalse( db.MayAccess( null, "test" ) );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void MultipleInsertTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
				TableColumn[] columns = new TableColumn[]
			{
				new TableColumn( "col1", ColumnType.Numeric ),
				new TableColumn( "col2", ColumnType.Text )
			};

				db.CreateTable( Plugin, "multiinserttest", columns );

				Dictionary<string, object>[] values = new Dictionary<string, object>[]
			{
				new Dictionary<string,object>{ {"col1", 1}, {"col2", "1"} },
				new Dictionary<string,object>{ {"col1", 2}, {"col2", "2"} },
				new Dictionary<string,object>{ {"col1", 3}, {"col2", "3"} },
			};

				Assert.AreEqual( 3, db.Insert( Plugin, "multiinserttest", values ) );

				IEnumerable<IDictionary<string, object>> result = db.Select( Plugin, "multiinserttest", new[] { "col1", "col2" } );
				Assert.AreEqual( 3, result.Count() );

				for( int i = 1; i <= 3; ++i )
				{
					IDictionary<string, object> row = result.Where( dict => Convert.ToInt32( dict["col1"] ) == i ).FirstOrDefault();
					Assert.IsNotNull( row );
					Assert.AreEqual( i.ToString(), row["col1"].ToString() );
					Assert.AreEqual( i.ToString(), row["col2"] );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void TransactionTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
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
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void WhereTest()
		{
			using( PluginDatabase db = new PluginDatabase( Connection ) )
			{
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

				Assert.AreEqual( 5, db.Update( Plugin, "wheretest", new Dictionary<string, object> { { "col2", "test" } }, where ) );

				IEnumerable<IDictionary<string, object>> result = db.Select( Plugin, "wheretest", new[] { "col2" }, where );
				Assert.AreEqual( 5, result.Count() );

				Assert.IsTrue( result.All( dict => dict.Values.All( v => v.Equals( "test" ) ) ) );

				db.Delete( Plugin, "wheretest", where );

				result = db.Select( Plugin, "wheretest", new[] { "col2" }, where );
				Assert.AreEqual( 0, result.Count() );
			}
		}
	}
}
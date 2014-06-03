// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WhereClause_Tests : TestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void EmptyWhereTest()
		{
			WhereClause where = new WhereClause();

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				Assert.AreEqual( string.Empty, where.ToSql( cmd ) );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void MultipleWhereTest()
		{
			WhereClause where = new WhereClause();
			where.AddCondition( "column", 123 );
			where.AddCondition( "second", "test", WhereOperation.NotEquals );
			where.AddCondition( "greater", 9, WhereOperation.Greater );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				string expected = "column = @where_column AND second != @where_second AND greater > @where_greater";

				Assert.AreEqual( expected, where.ToSql( cmd ) );

				Assert.AreEqual( 3, cmd.Parameters.Count );

				Assert.AreEqual( "where_column", cmd.Parameters[0].ParameterName );
				Assert.AreEqual( 123, cmd.Parameters[0].Value );

				Assert.AreEqual( "where_second", cmd.Parameters[1].ParameterName );
				Assert.AreEqual( "test", cmd.Parameters[1].Value );

				Assert.AreEqual( "where_greater", cmd.Parameters[2].ParameterName );
				Assert.AreEqual( 9, cmd.Parameters[2].Value );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void OpTest()
		{
			Dictionary<WhereOperation, string> expectedValues = new Dictionary<WhereOperation, string>();
			expectedValues.Add( WhereOperation.Equals, "=" );
			expectedValues.Add( WhereOperation.Greater, ">" );
			expectedValues.Add( WhereOperation.GreaterOrEqual, ">=" );
			expectedValues.Add( WhereOperation.Less, "<" );
			expectedValues.Add( WhereOperation.LessOrEqual, "<=" );
			expectedValues.Add( WhereOperation.NotEquals, "!=" );

			foreach( KeyValuePair<WhereOperation, string> kvp in expectedValues )
			{
				WhereClause where = new WhereClause();
				where.AddCondition( "col", 123, kvp.Key );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					Assert.AreEqual( string.Format( "col {0} @where_col", kvp.Value ), where.ToSql( cmd ) );
				}
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void SingleWhereTest()
		{
			WhereClause where = new WhereClause();
			where.AddCondition( "column", 123 );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				string expected = "column = @where_column";

				Assert.AreEqual( expected, where.ToSql( cmd ) );

				Assert.AreEqual( 1, cmd.Parameters.Count );
				Assert.AreEqual( "where_column", cmd.Parameters[0].ParameterName );
				Assert.AreEqual( 123, cmd.Parameters[0].Value );
			}
		}
	}
}
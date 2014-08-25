// $Id$

using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Blitzy.Plugin
{
	public interface IDatabase
	{
		DbTransaction BeginTransaction( IsolationLevel isolationLevel = IsolationLevel.ReadCommitted );

		bool CreateTable( IPlugin plugin, string tableName, TableColumn[] columns );

		void Delete( IPlugin plugin, string tableName, WhereClause where = null );

		void DropTable( IPlugin plugin, string tableName );

		int Insert( IPlugin plugin, string tableName, IDictionary<string, object> values );

		int Insert( IPlugin plugin, string tableName, IEnumerable<IDictionary<string, object>> values );

		IEnumerable<IDictionary<string, object>> Select( IPlugin plugin, string tableName, string[] columns, WhereClause where = null );

		int Update( IPlugin plugin, string tableName, IDictionary<string, object> newValues, WhereClause where = null );
	}
}
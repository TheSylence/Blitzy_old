// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin
{
	public interface IDatabase
	{
		DbTransaction BeginTransaction( global::System.Data.IsolationLevel isolationLevel );

		bool CreateTable( IPlugin plugin, string tableName, TableColumn[] columns );

		void Delete( IPlugin plugin, string tableName, WhereClause where = null );

		void DropTable( IPlugin plugin, string tableName );

		void Insert( IPlugin plugin, string tableName, IDictionary<string, object> values );

		IEnumerable<IDictionary<string, object>> Select( IPlugin plugin, string tableName, string[] columns, WhereClause where = null );

		void Update( IPlugin plugin, string tableName, IDictionary<string, object> newValues, WhereClause where = null );
	}
}
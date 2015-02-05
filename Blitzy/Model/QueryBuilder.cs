

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Blitzy.Model
{
	internal static class QueryBuilder
	{
		#region Methods

		internal static string CreateTable( string tableName, Dictionary<string, string> columns )
		{
			string columnList = string.Join( ",", columns.Keys.Select( c => c + " " + columns[c] ) );
			return "CREATE TABLE " + tableName + "(" + columnList + ");";
		}

		internal static string RenameTable( string oldName, string newName )
		{
			return string.Format( CultureInfo.InvariantCulture, "ALTER TABLE {0} RENAME TO {1};", oldName, newName );
		}

		internal static string DropTable( string table )
		{
			return string.Format( CultureInfo.InvariantCulture, "DROP TABLE {0};", table );
		}

		internal static string CopyTable( string source, string dest, IEnumerable<string> columns )
		{
			return CopyTable( source, dest, columns, columns );
		}

		internal static string CopyTable( string source, string dest, IEnumerable<string> sourceColumns, IEnumerable<string> destColumns )
		{
			string sourceColumnList = string.Join( ",", sourceColumns );
			string destColumnList = string.Join( ",", destColumns );

			string format = "INSERT INTO {1} ({3}) SELECT {2} FROM {0};";
			return string.Format( CultureInfo.InvariantCulture, format, source, dest, sourceColumnList, destColumnList );
		}

		#endregion Methods
	}
}
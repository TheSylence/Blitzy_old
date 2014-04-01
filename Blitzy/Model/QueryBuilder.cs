// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

		#endregion Methods
	}
}
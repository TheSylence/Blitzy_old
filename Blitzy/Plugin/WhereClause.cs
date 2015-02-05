using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace Blitzy.Plugin
{
	public enum WhereOperation
	{
		Equals,
		NotEquals,
		Greater,
		GreaterOrEqual,
		Less,
		LessOrEqual
	}

	public class WhereClause
	{
		public void AddCondition( string column, object value, WhereOperation op = WhereOperation.Equals )
		{
			Entries.Add( new WhereEntry( column, value, op ) );
		}

		internal string ToSql( DbCommand cmd )
		{
			return string.Join( " AND ", Entries.Select( e => e.ToSql( cmd ) ) );
		}

		private readonly List<WhereEntry> Entries = new List<WhereEntry>();

		private class WhereEntry
		{
			public WhereEntry( string column, object value, WhereOperation op )
			{
				Column = column;
				Value = value;
				Op = op;
			}

			public string ToSql( DbCommand cmd )
			{
				string name = string.Format( "where_{0}", Column );
				cmd.AddParameter( name, Value );
				return string.Format( "{0} {1} @{2}", Column, ToSql( Op ), name );
			}

			private static string ToSql( WhereOperation op )
			{
				switch( op )
				{
					case WhereOperation.Equals:
						return "=";

					case WhereOperation.NotEquals:
						return "!=";

					case WhereOperation.Less:
						return "<";

					case WhereOperation.LessOrEqual:
						return "<=";

					case WhereOperation.Greater:
						return ">";

					case WhereOperation.GreaterOrEqual:
						return ">=";

					default:
						throw new ArgumentException( "Unknown Where Operation" );
				}
			}

			private readonly string Column;
			private readonly WhereOperation Op;
			private readonly object Value;
		}
	}
}
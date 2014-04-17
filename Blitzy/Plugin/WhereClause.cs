// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
		#region Constructor

		#endregion Constructor

		#region Methods

		public void AddCondition( string column, object value, WhereOperation op = WhereOperation.Equals )
		{
			Entries.Add( new WhereEntry( column, value, op ) );
		}

		internal string ToSql( SQLiteCommand cmd )
		{
			// FIXME: Escape parameters and co
			return string.Join( " AND ", Entries.Select( e => e.ToSql( cmd ) ) );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private List<WhereEntry> Entries = new List<WhereEntry>();

		#endregion Attributes

		#region WhereEntry

		private class WhereEntry
		{
			public readonly string Column;
			public readonly WhereOperation Op;
			public readonly object Value;

			public WhereEntry( string column, object value, WhereOperation op )
			{
				Column = column;
				Value = value;
				Op = op;
			}

			public string ToSql( SQLiteCommand cmd )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = Value;
				param.ParameterName = string.Format( "where_{0}", Column );
				cmd.Parameters.Add( param );

				return string.Format( "{0} {1} @{2}", Column, ToSql( Op ), param.ParameterName );
			}

			private static string ToSql( WhereOperation Op )
			{
				switch( Op )
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
		}

		#endregion WhereEntry
	}
}
// $Id$

namespace Blitzy.Plugin
{
	public enum ColumnType
	{
		Numeric,
		Text
	}

	public struct TableColumn
	{
		#region Constructor

		public TableColumn( string name, ColumnType type, int length = 0, bool allowNull = false )
		{
			Name = name;
			Type = type;
			Length = length;
			AllowNull = allowNull;
		}

		#endregion Constructor

		#region Methods

		internal string ToSql()
		{
			string sqlType;
			if( Type == ColumnType.Text )
			{
				sqlType = Length == 0 ?
					"TEXT" :
					string.Format( "VARCHAR({0})", Length );
			}
			else
			{
				sqlType = "NUMERIC";
			}

			if( !AllowNull )
			{
				sqlType += " NOT NULL";
			}

			return string.Format( "[{0}] {1}", Name, sqlType );
		}

		#endregion Methods

		#region Fields

		public readonly bool AllowNull;
		public readonly int Length;
		public readonly string Name;
		public readonly ColumnType Type;

		#endregion Fields
	}
}
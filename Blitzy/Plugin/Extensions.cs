using System.Data.Common;

namespace Blitzy.Plugin
{
	public static class Extensions
	{
		public static void AddParameter( this DbCommand command, string name, object value )
		{
			DbParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add( parameter );
		}

		public static string ReadString( this DbDataReader reader, string columnName )
		{
			int ordinal = reader.GetOrdinal( columnName );
			if( reader.IsDBNull( ordinal ) )
			{
				return null;
			}

			return reader.GetString( ordinal );
		}

		public static bool TableExists( this DbConnection connection, string tableName )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE name=@tableName AND type='table';";
				cmd.AddParameter( "tableName", tableName );

				return (int)( cmd.ExecuteScalar() ) == 1;
			}
		}
	}
}
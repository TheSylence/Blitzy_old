using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class PluginDatabase : BaseObject, IDatabase
	{
		public PluginDatabase( DbConnection connection )
		{
			Connection = connection;
		}

		public DbTransaction BeginTransaction( IsolationLevel isolationLevel = IsolationLevel.ReadCommitted )
		{
			return Connection.BeginTransaction( isolationLevel );
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public bool CreateTable( IPlugin plugin, string tableName, TableColumn[] columns )
		{
			if( plugin == null )
			{
				return false;
			}

			try
			{
				tableName = GenerateTableName( plugin, tableName );
				string columnDefinitions = string.Join( ",", columns.Select( c => c.ToSql() ) );

				using( DbCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = string.Format( "CREATE TABLE [{0}] ( {1} );", tableName, columnDefinitions );

					cmd.ExecuteNonQuery();
				}

				using( DbCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO plugin_tables ( TableName, PluginID ) VALUES (@tableName, @pluginId);";
					cmd.AddParameter( "tableName", tableName );
					cmd.AddParameter( "pluginId", plugin.PluginID );

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}

				return true;
			}
			catch( Exception ex )
			{
				LogError( "Failed to create table for plugin {0}: {1}", plugin.Name, ex );
			}

			return false;
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public void Delete( IPlugin plugin, string tableName, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "DELETE FROM [{0}]", tableName );
				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public void DropTable( IPlugin plugin, string tableName )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "DROP TABLE [{0}];", tableName );
				cmd.ExecuteNonQuery();
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM plugin_tables WHERE TableName = @tableName AND PluginID = @pluginId";
				cmd.AddParameter( "tableName", tableName );
				cmd.AddParameter( "pluginId", plugin.PluginID );

				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public int Insert( IPlugin plugin, string tableName, IDictionary<string, object> values )
		{
			return Insert( plugin, tableName, new[] { values } );
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public int Insert( IPlugin plugin, string tableName, IEnumerable<IDictionary<string, object>> values )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return 0;
			}

			string columns = string.Join( "],[", values.First().Select( v => v.Key ).OrderBy( k => k ) );

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				int i = 0;

				string columnValues = string.Join( "),(", values.Select( row =>
					{
						++i;
						return string.Join( ",", row.OrderBy( k => k.Key ).Select( kvp =>
						{
							string name = string.Format( "value_{0}_{1}", kvp.Key, i );
							cmd.AddParameter( name, kvp.Value );

							return string.Format( "@{0}", name );
						} ) );
					} ) );

				cmd.CommandText = string.Format( "INSERT INTO [{0}] ([{1}]) VALUES ({2});", tableName, columns, columnValues );

				return cmd.ExecuteNonQuery();
			}
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public IEnumerable<IDictionary<string, object>> Select( IPlugin plugin, string tableName, string[] columns, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				yield break;
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "SELECT [{1}] FROM [{0}]", tableName, string.Join( "],[", columns ) );
				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Dictionary<string, object> values = new Dictionary<string, object>();
						for( int i = 0; i < reader.VisibleFieldCount; ++i )
						{
							values.Add( reader.GetName( i ), reader.GetValue( i ) );
						}

						yield return values;
					}
				}
			}
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities",
			Justification = "Values are escpaed an we have tests for this" )]
		public int Update( IPlugin plugin, string tableName, IDictionary<string, object> newValues, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return 0;
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				string values = string.Join( ",", newValues.Select( kvp =>
				{
					string name = string.Format( "value_{0}", kvp.Key );
					cmd.AddParameter( name, kvp.Value );

					return string.Format( "[{0}] = @{1}", kvp.Key, name );
				} ) );

				cmd.CommandText = string.Format( "UPDATE [{0}] SET {1}", tableName, values );

				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();
				return cmd.ExecuteNonQuery();
			}
		}

		internal bool MayAccess( IPlugin plugin, string tableName )
		{
			if( plugin == null || string.IsNullOrWhiteSpace( tableName ) )
			{
				return false;
			}

			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT PluginID FROM plugin_tables WHERE TableName = @tableName";
				cmd.AddParameter( "tableName", tableName );

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						return false;
					}

					return plugin.PluginID.Equals( reader.GetGuid( 0 ) );
				}
			}
		}

		private static string GenerateTableName( IPlugin plugin, string tableName )
		{
			tableName = string.Format( "{0}_{1}", plugin.Name, tableName );

			char[] replacements = { '[', ']', ';', '\"', '\'', ':' };

			return replacements.Aggregate( tableName, ( current, c ) => current.Replace( c, '_' ) );
		}

		private readonly DbConnection Connection;
	}
}
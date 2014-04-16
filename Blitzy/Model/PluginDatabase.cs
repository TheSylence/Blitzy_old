// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class PluginDatabase : BaseObject, IDatabase
	{
		#region Constructor

		public PluginDatabase( SQLiteConnection connection )
		{
			Connection = connection;
		}

		#endregion Constructor

		#region Methods

		public System.Data.Common.DbTransaction BeginTransaction( System.Data.IsolationLevel isolationLevel = System.Data.IsolationLevel.ReadCommitted )
		{
			return Connection.BeginTransaction( isolationLevel );
		}

		public bool CreateTable( IPlugin plugin, string table, TableColumn[] columns )
		{
			if( plugin == null )
			{
				return false;
			}

			try
			{
				string tableName = string.Format( "{0}_{1}", plugin.Name, table );
				string columnDefinitions = string.Join( ",", columns.Select( c => c.ToSql() ) );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = string.Format( "CREATE TABLE {0} ( {1} );", tableName, columnDefinitions );

					cmd.ExecuteNonQuery();
				}

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "INSERT INTO plugin_tables ( TableName, PluginID ) VALUES (@tableName, @pluginId);";

					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "tableName";
					param.Value = tableName;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "pluginId";
					param.Value = plugin.PluginID;
					cmd.Parameters.Add( param );

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

		public void Delete( IPlugin plugin, string tableName, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "DELETE FROM {0}", tableName );
				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}
		}

		public void DropTable( IPlugin plugin, string tableName )
		{
			tableName = GenerateTableName( plugin, tableName ); ;

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "DROP TABLE {0};", tableName );
				cmd.ExecuteNonQuery();
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "DELETE FROM plugin_tables WHERE TableName = @tableName AND PluginID = @pluginId";

				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "tableName";
				param.Value = tableName;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "pluginId";
				param.Value = plugin.PluginID;
				cmd.Parameters.Add( param );

				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public void Insert( IPlugin plugin, string tableName, IDictionary<string, object> values )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			string columns = string.Join( ",", values.Select( v => v.Key ).OrderBy( k => k ) );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				string columnValues = string.Join( ",", values.OrderBy( k => k.Key ).Select( kvp =>
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = string.Format( "value_{0}", kvp.Key );
					param.Value = kvp.Value;
					cmd.Parameters.Add( param );

					return string.Format( "@{0}", param.ParameterName );
				} ) );

				cmd.CommandText = string.Format( "INSERT INTO {0} ({1}) VALUES({2});", tableName, columns, columnValues );

				cmd.ExecuteNonQuery();
			}
		}

		public IEnumerable<IDictionary<string, object>> Select( IPlugin plugin, string tableName, string[] columns, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				yield break;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = string.Format( "SELECT {1} FROM {0}", tableName, string.Join( ",", columns ) );
				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
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

		public void Update( IPlugin plugin, string tableName, IDictionary<string, object> newValues, WhereClause where = null )
		{
			tableName = GenerateTableName( plugin, tableName );

			if( !MayAccess( plugin, tableName ) )
			{
				return;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				string values = string.Join( ",", newValues.Select( kvp =>
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.Value = kvp.Value;
					param.ParameterName = string.Format( "value_{0}", kvp.Key );
					cmd.Parameters.Add( param );

					return string.Format( "{0} = @{1}", kvp.Key, param.ParameterName );
				} ) );

				cmd.CommandText = string.Format( "UPDATE {0} SET {1}", tableName, values );

				if( where != null )
				{
					cmd.CommandText += " WHERE " + where.ToSql( cmd );
				}

				cmd.CommandText += ";";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}
		}

		private static string GenerateTableName( IPlugin plugin, string tableName )
		{
			tableName = string.Format( "{0}_{1}", plugin.Name, tableName );
			return tableName;
		}

		private bool MayAccess( IPlugin plugin, string tableName )
		{
			if( plugin == null || string.IsNullOrWhiteSpace( tableName ) )
			{
				return false;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT PluginID FROM plugin_tables WHERE TableName = @tableName";

				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "tableName";
				param.Value = tableName;
				cmd.Parameters.Add( param );

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						return false;
					}

					return plugin.PluginID.Equals( reader.GetGuid( 0 ) );
				}
			}
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private SQLiteConnection Connection;

		#endregion Attributes
	}
}
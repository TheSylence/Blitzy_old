using System;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Blitzy.Model;

namespace Blitzy
{
	public class DbConnectionFactory
	{
		public DbConnectionFactory()
		{
			SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder
			{
				JournalMode = SQLiteJournalModeEnum.Wal,
				Pooling = true
			};

			connectionStringBuilder.DataSource = Path.Combine( Constants.DataPath, Constants.DataFileName );

			ConnectionString = connectionStringBuilder.ToString();
		}

		internal virtual DbConnection OpenConnection()
		{
			DbConnection connection = new SQLiteConnection( ConnectionString );
			connection.Open();

			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "PRAGMA user_version;";
				object val = cmd.ExecuteScalar();
				int version = Convert.ToInt32( val );

				if( version == 0 )
				{
					LogHelper.LogInfo( this, "Creating database structure for first time launch" );

					DatabaseCreator.CreateDatabase( connection );
				}
				else
				{
					Existed = true;
				}

				using( DatabaseUpgrader upgrader = new DatabaseUpgrader() )
				{
					upgrader.UpgradeDatabase( version, connection );
				}
			}

			return connection;
		}

		internal bool Existed { get; private set; }

		private string ConnectionString;
	}
}
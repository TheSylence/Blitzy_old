using System;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Blitzy.Model
{
	internal class Database : BaseObject
	{
		public Database( DbConnection connection = null )
		{
			if( connection != null )
			{
				Connection = connection;
				Existed = true;
			}
			else
			{
				SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder
				{
					JournalMode = SQLiteJournalModeEnum.Wal,
					Pooling = true
				};

				connectionStringBuilder.DataSource = Path.Combine( Constants.DataPath, Constants.DataFileName );
				Existed = File.Exists( connectionStringBuilder.DataSource );

				Connection = ToDispose( new SQLiteConnection( connectionStringBuilder.ToString() ) );
				Connection.Open();
			}
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
		internal bool CheckExistance()
		{
			if( !Existed )
			{
				LogInfo( "Creating database structure for first time launch" );

				DatabaseCreator.CreateDatabase( Connection );
			}
			else
			{
				using( DbCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "PRAGMA user_version;";
					object val = cmd.ExecuteScalar();

					LogInfo( "Database Version: {0}", val );

					using( DatabaseUpgrader upgrader = new DatabaseUpgrader() )
					{
						upgrader.UpgradeDatabase( Convert.ToInt32( val ), Connection );
					}
				}
			}

			return Existed;
		}

		internal DbConnection Connection { get; private set; }

		private readonly bool Existed;
	}
}
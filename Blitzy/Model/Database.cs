// $Id$

using System;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Blitzy.Model
{
	internal class Database : BaseObject
	{
		#region Constructor

		public Database()
		{
			SQLiteConnectionStringBuilder connectionStringBuilder = new SQLiteConnectionStringBuilder
			{
				JournalMode = SQLiteJournalModeEnum.Wal
			};

			if( RuntimeConfig.Tests )
			{
				connectionStringBuilder.FullUri = ":memory:";
				Existed = false;
			}
			else
			{
				connectionStringBuilder.DataSource = Path.Combine( Constants.DataPath, Constants.DataFileName );
				Existed = File.Exists( connectionStringBuilder.DataSource );
			}

			Connection = ToDispose( new SQLiteConnection( connectionStringBuilder.ToString() ) );
			Connection.Open();
		}

		#endregion Constructor

		#region Methods

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
				using( SQLiteCommand cmd = Connection.CreateCommand() )
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

		#endregion Methods

		#region Properties

		internal SQLiteConnection Connection { get; private set; }

		#endregion Properties

		#region Attributes

		private readonly bool Existed;

		#endregion Attributes
	}
}
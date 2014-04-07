﻿// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	internal class Database : BaseObject, IDisposable
	{
		#region Constructor

		public Database()
		{
			SQLiteConnectionStringBuilder connectionSB = new SQLiteConnectionStringBuilder();
			connectionSB.DataSource = Path.Combine( Constants.DataPath, Constants.DataFileName );
			connectionSB.JournalMode = SQLiteJournalModeEnum.Wal;

			Existed = File.Exists( connectionSB.DataSource );
			Connection = ToDispose( new SQLiteConnection( connectionSB.ToString() ) );
			Connection.Open();
		}

		#endregion Constructor

		#region Methods

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
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

		private bool Existed;

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	internal class Database : LogObject, IDisposable
	{
		#region Constructor

		public Database()
		{
			ConnectionSB = new SQLiteConnectionStringBuilder();
			ConnectionSB.DataSource = Path.Combine( Constants.DataPath, Constants.DataFileName );

			Connection = new SQLiteConnection( ConnectionSB.ToString() );
			Connection.Open();
		}

		#endregion Constructor

		#region Disposable

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Database"/> is reclaimed by garbage collection.
		/// </summary>
		~Database()
		{
			Dispose( false );
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected virtual void Dispose( bool managed )
		{
			if( managed )
			{
				Connection.Dispose();
			}
		}

		#endregion Disposable

		#region Methods

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
		internal bool CheckExistance()
		{
			bool existed = File.Exists( ConnectionSB.DataSource );
			if( existed )
			{
				LogInfo( "Database exists, checking if database is empty" );
				FileInfo inf = new FileInfo( ConnectionSB.DataSource );
				existed = inf.Length != 0;

				if( !existed )
				{
					LogInfo( "Database is empty" );
				}
				else
				{
					LogInfo( "Database is not empty. Skipping first time initialization" );
				}
			}

			if( !existed )
			{
				LogInfo( "Creating database structure for first time launch" );

				StringBuilder sb = new StringBuilder();
				sb.Append( "BEGIN TRANSACTION;" );

				sb.Append( QueryBuilder.CreateTable( "folders", new Dictionary<string, string>
				{
					{ "FolderID", "INTEGER PRIMARY KEY" },
					{ "Path", "TEXT NOT NULL" },
					{ "Recursive", "INTEGER NOT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "folder_rules", new Dictionary<string, string>
				{
					{ "FolderID", "INTEGER NOT NULL" },
					{ "Rule", "TEXT NOT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "folder_excludes", new Dictionary<string, string>
				{
					{ "FolderID", "INTEGER NOT NULL" },
					{ "Exclude", "TEXT NOT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "settings", new Dictionary<string, string>
				{
					{ "[Key]", "VARCHAR(255)" },
					{ "[Value]", "TEXT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "history", new Dictionary<string, string>
				{
					{ "HistoryID", "INTEGER PRIMARY KEY" },
					{ "Command", "TEXT NOT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "plugins", new Dictionary<string, string>
				{
					{ "PluginID", "VARCHAR(40) PRIMARY KEY" },
					{ "Version", "VARCHAR(32) NOT NULL" },
					{ "Disabled", "INTEGER DEFAULT '0' NOT NULL" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "commands", new Dictionary<string, string>
				{
					{ "Name", "TEXT NOT NULL" },
					{ "Plugin", "VARCHAR(40) NOT NULL" },
					{ "ExectionCount", "INTEGER NOT NULL" },

					{ "PRIMARY KEY", "([Name],[Plugin])" }
				} ) );

				sb.Append( QueryBuilder.CreateTable( "files", new Dictionary<string, string>
				{
					{ "Command", "TEXT NOT NULL" },
					{ "Name", "VARCHAR(255) NOT NULL" },
					{ "Icon", "TEXT NULL" },
					{ "[Type]", "VARCHAR(10) NULL" },
					{ "Arguments", "TEXT NULL" },

					{ "PRIMARY KEY", "([Command],[Arguments],[Name])" },
				} ) );

				sb.AppendFormat( "PRAGMA user_version = {0};", DatabaseUpgrader.DatabaseVersion );
				sb.Append( "COMMIT;" );

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = sb.ToString();
					cmd.ExecuteNonQuery();
				}
			}
			else
			{
				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					cmd.CommandText = "PRAGMA user_version;";
					object val = cmd.ExecuteScalar();

					LogInfo( "Database Version: {0}", val );

					DatabaseUpgrader upgrader = new DatabaseUpgrader();
					upgrader.UpgradeDatabase( Convert.ToInt32( val ), Connection );
				}
			}

			return existed;
		}

		internal void ResetExecutionCount()
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "UPDATE Blitzy_Commands SET ExecutionCount = 0";
				cmd.ExecuteNonQuery();
			}
		}

		internal void UpdateExecutionCount( string itemName, string pluginId )
		{
			// Check if command was executed before
			bool registered = false;
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT ExecutionCount FROM Blitzy_Commands WHERE Plugin = @plugin AND Name = @name;";
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "name";
				param.Value = itemName;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "plugin";
				param.Value = pluginId;
				cmd.Parameters.Add( param );

				registered = cmd.ExecuteScalar() != null;
			}

			// Update execution count of the command
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "name";
				param.Value = itemName;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "plugin";
				param.Value = pluginId;
				cmd.Parameters.Add( param );

				if( registered )
				{
					cmd.CommandText = "UPDATE Blitzy_Commands SET ExecutionCount = ExecutionCount + 1 WHERE Plugin = @plugin AND Name = @name;";
				}
				else
				{
					cmd.CommandText = "INSERT INTO Blitzy_Commands (Plugin, Name, ExecutionCount) VALUES (@plugin, @name, 1);";
				}

				cmd.ExecuteNonQuery();
			}
		}

		#endregion Methods

		#region Properties

		internal SQLiteConnection Connection { get; private set; }

		#endregion Properties

		#region Attributes

		private SQLiteConnectionStringBuilder ConnectionSB;

		#endregion Attributes
	}
}
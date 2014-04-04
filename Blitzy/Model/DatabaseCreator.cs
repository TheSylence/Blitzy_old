// $Id$

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	internal static class DatabaseCreator
	{
		internal static void CreateDatabase( SQLiteConnection connection )
		{
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

			sb.Append( QueryBuilder.CreateTable( "plugin_settings", new Dictionary<string, string>
			{
				{ "PluginID", "VARCHAR(40)" },
				{ "[Key]", "VARCHAR(255)" },
				{ "[Value]", "TEXT NULL" },

				{ "PRIMARY KEY", "([PluginID],[Key])" }
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

			sb.Append( QueryBuilder.CreateTable( "weby", new Dictionary<string, string>
				{
					{ "WebyID", "INTEGER PRIMARY KEY" },
					{ "Name", "VARCHAR(50) NOT NULL" },
					{ "Description", "VARCHAR(255) NOT NULL" },
					{ "Url", "TEXT NOT NULL" },
				} ) );

			Type type = typeof( SystemSetting );
			foreach( SystemSetting setting in Enum.GetValues( type ) )
			{
				MemberInfo member = type.GetMember( setting.ToString() ).First();
				object defaultValue = member.GetCustomAttribute<DefaultValueAttribute>().Value;

				sb.AppendFormat( "INSERT INTO settings ([Key], [Value]) VALUES( '{0}', '{1}' );", setting.ToString(), defaultValue.ToString() );
			}

			sb.AppendFormat( "PRAGMA user_version = {0};", DatabaseUpgrader.DatabaseVersion );
			sb.Append( "COMMIT;" );

			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = sb.ToString();
				cmd.ExecuteNonQuery();
			}
		}
	}
}
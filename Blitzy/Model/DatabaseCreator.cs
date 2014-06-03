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
		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security",
			"CA2100:Review SQL queries for security vulnerabilities", Justification = "String is not affacted by user input" )]
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
				{ "ExecutionCount", "INTEGER NOT NULL" },

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

			sb.Append( QueryBuilder.CreateTable( "workspaces", new Dictionary<string, string>
			{
				{ "WorkspaceID", "INTEGER PRIMARY KEY" },
				{ "Name", "TEXT NOT NULL" }
			} ) );

			sb.Append( QueryBuilder.CreateTable( "workspace_items", new Dictionary<string, string>
			{
				{ "ItemID", "INTEGER PRIMARY KEY" },
				{ "WorkspaceID", "INTEGER NOT NULL" },
				{ "ItemOrder", "INTEGER NOT NULL" },
				{ "ItemCommand", "TEXT NOT NULL" }
			} ) );

			sb.Append( QueryBuilder.CreateTable( "plugin_tables", new Dictionary<string, string>
			{
				{ "TableName", "VARCHAR(255) PRIMARY KEY" },
				{ "PluginID", "VARCHAR(40) NOT NULL" },
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
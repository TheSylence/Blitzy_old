// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;

namespace Blitzy.Model
{
	internal class FileEntry : IEquatable<FileEntry>
	{
		#region Constructor

		public FileEntry( string command, string name, string icon, string type, string args )
		{
			Command = command;
			Name = name;
			Icon = icon;
			Type = type;
			Arguments = args;
		}

		#endregion Constructor

		#region Methods

		public bool Equals( FileEntry other )
		{
			if( other == null )
			{
				return false;
			}

			return Command.Equals( other.Command, StringComparison.OrdinalIgnoreCase ) && Arguments.Equals( other.Arguments, StringComparison.OrdinalIgnoreCase );
		}

		public override bool Equals( object obj )
		{
			return Equals( obj as FileEntry );
		}

		public override int GetHashCode()
		{
			int hash = 17;
			hash = hash * 23 + Command.GetHashCode();
			hash = hash * 23 + Arguments.GetHashCode();

			return hash;
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security",
			"CA2100:Review SQL queries for security vulnerabilities", Justification = "Query is prepared" )]
		internal static void CreateBatchStatement( SQLiteCommand cmd, IEnumerable<FileEntry> entries )
		{
			StringBuilder sb = new StringBuilder();

			int cnt = 0;
			sb.Append( "INSERT INTO files ([Command], [Name], [Icon], [Type], [Arguments]) VALUES " );
			sb.Append( string.Join( ",", entries.Select( entry =>
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "cmd" + cnt.ToString();
					param.Value = entry.Command;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "name" + cnt.ToString();
					param.Value = entry.Name;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "icon" + cnt.ToString();
					param.Value = entry.Icon;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "type" + cnt.ToString();
					param.Value = entry.Type;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "args" + cnt.ToString();
					param.Value = entry.Arguments;
					cmd.Parameters.Add( param );

					string ret = string.Format( "(@cmd{0}, @name{0}, @icon{0}, @type{0}, @args{0})", cnt );
					cnt++;
					return ret;
				} ) ) );

			cmd.CommandText = sb.ToString();
		}

		#endregion Methods

		#region Properties

		internal string Arguments { get; private set; }

		internal string Command { get; private set; }

		internal string Icon { get; private set; }

		internal string Name { get; private set; }

		internal string Type { get; private set; }

		#endregion Properties

		#region Attributes

		internal const int ParameterCount = 5;

		#endregion Attributes
	}
}
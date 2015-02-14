using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class FileEntry : IEquatable<FileEntry>
	{
		public FileEntry( string command, string name, string icon, string type, string args )
		{
			Command = command;
			Name = name;
			Icon = icon;
			Type = type;
			Arguments = args;
		}

		public bool Equals( FileEntry other )
		{
			if( other == null )
			{
				return false;
			}

			return GetHashCode() == other.GetHashCode();
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

		[SuppressMessage( "Microsoft.Security",
			"CA2100:Review SQL queries for security vulnerabilities", Justification = "Query is prepared" )]
		internal static void CreateBatchStatement( DbCommand cmd, IEnumerable<FileEntry> entries )
		{
			StringBuilder sb = new StringBuilder();

			int cnt = 0;
			sb.Append( "INSERT INTO files ([Command], [Name], [Icon], [Type], [Arguments]) VALUES " );
			sb.Append( string.Join( ",", entries.Select( entry =>
			{
				cmd.AddParameter( "cmd" + cnt.ToString( CultureInfo.InvariantCulture ), entry.Command );
				cmd.AddParameter( "name" + cnt.ToString( CultureInfo.InvariantCulture ), entry.Name );
				cmd.AddParameter( "icon" + cnt.ToString( CultureInfo.InvariantCulture ), entry.Icon );
				cmd.AddParameter( "type" + cnt.ToString( CultureInfo.InvariantCulture ), entry.Type );
				cmd.AddParameter( "args" + cnt.ToString( CultureInfo.InvariantCulture ), entry.Arguments );

				string ret = string.Format( "(@cmd{0}, @name{0}, @icon{0}, @type{0}, @args{0})", cnt );
				cnt++;
				return ret;
			} ) ) );

			cmd.CommandText = sb.ToString();
		}

		internal string Arguments { get; private set; }

		internal string Command { get; private set; }

		internal string Icon { get; private set; }

		internal string Name { get; private set; }

		internal string Type { get; private set; }

		internal const int ParameterCount = 5;
	}
}
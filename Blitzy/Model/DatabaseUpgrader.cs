// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	internal class DatabaseUpgrader : BaseObject
	{
		internal const int DatabaseVersion = 0;
		private List<string[]> Queries = new List<string[]>();

		internal DatabaseUpgrader()
		{
			Queries.Add( new string[] { string.Empty } );
		}

		[SuppressMessage( "Microsoft.Security", "CA2100" )]
		[SuppressMessage( "Microsoft.Performance", "CA1811", Justification = "This is just plain bullshit" )]
		internal void UpgradeDatabase( int oldVersion, SQLiteConnection db )
		{
			if( oldVersion > DatabaseVersion )
			{
				LogWarning( "Database version is newer than maximum supported version... this should not be !?" );
			}
			else if( oldVersion == DatabaseVersion )
			{
				return;
			}
			else
			{
				for( int i = oldVersion + 1; i <= DatabaseVersion; ++i )
				{
					LogInfo( "Upgrading Database to version {0}", i );

					foreach( string query in Queries[i] )
					{
						using( SQLiteCommand cmd = db.CreateCommand() )
						{
							cmd.CommandText = query;
							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}
	}
}
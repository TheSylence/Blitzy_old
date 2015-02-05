using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.Diagnostics.CodeAnalysis;

namespace Blitzy.Model
{
	internal class DatabaseUpgrader : BaseObject
	{
		internal DatabaseUpgrader()
		{
			Queries.Add( new[] { string.Empty } );
			Queries.Add( new[]
			{
				QueryBuilder.RenameTable( "weby_websites", "weby_websites_copy" ),
				QueryBuilder.CreateTable( "weby_websites", new Dictionary<string, string>
				{
					{ "WebyID", "INTEGER PRIMARY KEY" },
					{ "Name", "VARCHAR(50) NOT NULL " },
					{ "Description", "VARCHAR(255) NOT NULL " },
					{ "Url", "TEXT NOT NULL " },
					{ "Icon", "TEXT" }
				} ),
				QueryBuilder.CopyTable( "weby_websites_copy", "weby_websites", new[]{"WebyID","Name", "Description","Url","Icon"}),
				QueryBuilder.DropTable("weby_websites_copy")
			} );
		}

		[SuppressMessage( "Microsoft.Security", "CA2100" )]
		[SuppressMessage( "Microsoft.Performance", "CA1811", Justification = "This is just plain bullshit" )]
		internal void UpgradeDatabase( int oldVersion, SQLiteConnection db )
		{
			if( oldVersion > DatabaseVersion )
			{
				LogWarning( "Database version is newer than maximum supported version... this should not be !? Old: {0}, Db: {1}", oldVersion, DatabaseVersion );
			}
			else if( oldVersion < DatabaseVersion )
			{
				for( int i = oldVersion + 1; i <= DatabaseVersion; ++i )
				{
					LogInfo( "Upgrading Database to version {0}", i );

					foreach( string query in Queries[i] )
					{
						using( DbCommand cmd = db.CreateCommand() )
						{
							cmd.CommandText = query;
							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		internal const int DatabaseVersion = 0;
		private readonly List<string[]> Queries = new List<string[]>();
	}
}
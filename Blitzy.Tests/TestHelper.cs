using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using Blitzy.Model;
using GalaSoft.MvvmLight.Threading;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public sealed class TestHelper
	{
		[AssemblyCleanup()]
		public static void AssemblyCleanup()
		{
			while( TestFolders.Count > 0 )
			{
				string folder = TestFolders.Pop();
				Directory.Delete( folder, true );
			}

			Connection.Close();
			Connection.Dispose();
		}

		[AssemblyInitialize()]
		public static void AssemblyInit( TestContext context )
		{
			TestFolders = new Stack<string>();

			DispatcherHelper.Initialize();
			BasicConfigurator.Configure();
			Connection = CreateConnection();

			DatabaseCreator.CreateDatabase( Connection );
			CreatePluginTables();
		}

		public static void CreateTestFolder( string folder )
		{
			TestFolders.Push( folder );
			Directory.CreateDirectory( folder );
		}

		internal static int NextID()
		{
			lock( LockObject )
			{
				return ++IDCounter;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
		private static SQLiteConnection CreateConnection()
		{
			SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
			sb.FullUri = ":memory:";

			SQLiteConnection connection = new SQLiteConnection( sb.ToString() );
			connection.Open();
			return connection;
		}

		private static void CreatePluginTables()
		{
			using( DbCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = QueryBuilder.CreateTable( "weby_websites", new Dictionary<string, string>
				{
					{ "WebyID", "INTEGER PRIMARY KEY" },
					{ "Name", "VARCHAR(50) NOT NULL" },
					{ "Description", "VARCHAR(255) NOT NULL" },
					{ "Url", "TEXT NOT NULL" },
					{ "Icon", "TEXT" }
				} );

				cmd.ExecuteNonQuery();
			}
		}

		internal static SQLiteConnection Connection;
		private static int IDCounter = 123;
		private static object LockObject = new object();
		private static Stack<string> TestFolders;
	}
}
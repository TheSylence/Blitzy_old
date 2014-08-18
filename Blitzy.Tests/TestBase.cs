// $Id$

using System.Collections.Generic;
using System.Data.SQLite;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	public enum NativeMethodsType
	{
		Real,
		Test
	}

	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class TestBase
	{
		protected SQLiteConnection Connection { get; private set; }

		protected NativeMethodsMock NativeMethods { get; private set; }

		[TestCleanup]
		public virtual void AfterTestRun()
		{
			Connection.Close();
			Connection.Dispose();

			DialogServiceManager.Clear();
			Messenger.Reset();
		}

		[TestInitialize]
		public virtual void BeforeTestRun()
		{
			Messenger.OverrideDefault( new MockMessenger() );

			NativeMethods = new NativeMethodsMock();
			SetNativeMethods( NativeMethodsType.Real );
			RuntimeConfig.Tests = true;
			DispatcherHelper.Initialize();
			BasicConfigurator.Configure();
			Connection = CreateConnection();

			DatabaseCreator.CreateDatabase( Connection );

			//CreatePluginTables();
		}

		protected virtual void CreatePluginTables()
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
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

		protected void SetNativeMethods( NativeMethodsType type )
		{
			switch( type )
			{
				case NativeMethodsType.Real:
					INativeMethods.Instance = new NativeMethods();
					break;

				case NativeMethodsType.Test:
					INativeMethods.Instance = NativeMethods;
					break;
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Reliability", "CA2000:Dispose objects before losing scope" )]
		private SQLiteConnection CreateConnection()
		{
			SQLiteConnectionStringBuilder sb = new SQLiteConnectionStringBuilder();
			sb.FullUri = ":memory:";

			SQLiteConnection connection = new SQLiteConnection( sb.ToString() );
			connection.Open();
			return connection;
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Blitzy.ViewServices;
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

			DialogServiceManager.Clear();
		}

		[TestInitialize]
		public virtual void BeforeTestRun()
		{
			NativeMethods = new NativeMethodsMock();
			SetNativeMethods( NativeMethodsType.Real );
			RuntimeConfig.Tests = true;
			DispatcherHelper.Initialize();
			BasicConfigurator.Configure();
			Connection = CreateConnection();

			DatabaseCreator.CreateDatabase( Connection );
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
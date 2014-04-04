﻿// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.ViewServices;
using log4net.Config;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	[TestClass]
	public class TestBase
	{
		protected SQLiteConnection Connection { get; private set; }

		[TestCleanup]
		public virtual void AfterTestRun()
		{
			Connection.Close();

			DialogServiceManager.Clear();
		}

		[TestInitialize]
		public virtual void BeforeTestRun()
		{
			BasicConfigurator.Configure();
			Connection = CreateConnection();

			DatabaseCreator.CreateDatabase( Connection );
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
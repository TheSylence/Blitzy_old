using System.Collections.Generic;
using System.Data.Common;
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
		[TestCleanup]
		public virtual void AfterTestRun()
		{
		}

		[TestInitialize]
		public virtual void BeforeTestRun()
		{
			NativeMethods = new NativeMethodsMock();
			SetNativeMethods( NativeMethodsType.Real );
			RuntimeConfig.Tests = true;

			//CreatePluginTables();
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

		protected DbConnection Connection
		{
			get
			{
				return new ConnectionWrapper( TestHelper.Connection );
			}
		}

		protected NativeMethodsMock NativeMethods { get; private set; }
	}
}
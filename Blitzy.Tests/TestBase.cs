﻿using System.Data.Common;
using Blitzy.Tests.Mocks;
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

		protected DbConnectionFactory ConnectionFactory
		{
			get
			{
				return new TestConnectionFactory();
			}
		}

		protected NativeMethodsMock NativeMethods { get; private set; }
	}
}
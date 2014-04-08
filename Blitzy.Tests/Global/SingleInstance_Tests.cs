// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	public class SingleInstance_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void StartTest()
		{
			Assert.IsTrue( SingleInstance.Start() );
			Assert.IsFalse( SingleInstance.Start() );
			SingleInstance.Stop();
		}
	}
}
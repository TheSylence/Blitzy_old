// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CloseViewEventArgs_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void DefaultTest()
		{
			Assert.AreEqual( null, CloseViewEventArgs.Default.Result );
		}
	}
}
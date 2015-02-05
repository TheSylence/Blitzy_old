

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Utility
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class STAThread_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" )]
		public void QueueActionTest()
		{
			bool started = false;
			STAThread.QueueAction( () =>
			{
				started = true;
				Assert.AreEqual( ApartmentState.STA, Thread.CurrentThread.GetApartmentState() );
			} );

			Thread.Sleep( 100 );
			STAThread.Stop();

			Assert.IsTrue( started );
		}
	}
}
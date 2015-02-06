using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Utility
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class ProgressStatistic_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" )]
		public void ProgressChangeTest()
		{
			ProgressStatistic stat = new ProgressStatistic();
			Assert.IsFalse( stat.HasStarted );
			Assert.IsFalse( stat.HasFinished );
			Assert.IsFalse( stat.IsRunning );

			stat.ProgressChange( 123, 1234 );

			Assert.IsTrue( stat.HasStarted );
			Assert.IsFalse( stat.HasFinished );
			Assert.IsTrue( stat.IsRunning );

			stat.ProgressChange( 1234, 1234 );
			Assert.IsTrue( stat.HasStarted );
			Assert.IsTrue( stat.HasFinished );
			Assert.IsFalse( stat.IsRunning );

			stat = new ProgressStatistic();
			stat.ProgressChange( 123, 444 );
			ExceptionAssert.Throws<ArgumentException>( () => stat.ProgressChange( 100, 444 ) );

			stat = new ProgressStatistic();
			stat.Finish();
			ExceptionAssert.Throws<InvalidOperationException>( () => stat.ProgressChange( 123, 1234 ) );
		}
	}
}
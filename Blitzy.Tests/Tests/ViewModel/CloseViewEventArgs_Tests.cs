using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CloseViewEventArgs_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void DefaultTest()
		{
			Assert.AreEqual( null, CloseViewEventArgs.Default.Result );
		}
	}
}
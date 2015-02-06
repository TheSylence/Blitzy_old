

using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class SingleInstance_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" )]
		public void StartTest()
		{
			Assert.IsTrue( SingleInstance.Start() );
			Assert.IsFalse( SingleInstance.Start() );
			SingleInstance.Stop();
		}
	}
}
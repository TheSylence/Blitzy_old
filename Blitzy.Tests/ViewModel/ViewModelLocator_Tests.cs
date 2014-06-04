// $Id$

using System.Reflection;
using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ViewModelLocator_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void VMTest()
		{
			ViewModelLocator vm = new ViewModelLocator();
			foreach( PropertyInfo info in vm.GetType().GetProperties() )
			{
				Assert.IsNotNull( info.GetValue( vm ) );
			}
		}
	}
}
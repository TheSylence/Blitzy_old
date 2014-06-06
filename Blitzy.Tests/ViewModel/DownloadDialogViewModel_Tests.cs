// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewModel.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class DownloadDialogViewModel_Tests : TestBase
	{
		[TestMethod]
		public void PropertyChangedTest()
		{
			DownloadDialogViewModel vm = new DownloadDialogViewModel();
			PropertyChangedListener listener = new PropertyChangedListener( vm );
			Assert.IsTrue( listener.TestProperties() );
		}
	}
}
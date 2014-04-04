// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class MainViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void SettingsTest()
		{
			MainViewModel vm = new MainViewModel();

			Assert.IsTrue( vm.SettingsCommand.CanExecute( null ) );

			CallCheckServiceMock mock = new CallCheckServiceMock();
			DialogServiceManager.RegisterService( typeof( SettingsService ), mock );

			vm.SettingsCommand.Execute( null );

			Assert.IsTrue( mock.WasCalled );
			Assert.IsNotNull( mock.Parameter );
		}
	}
}
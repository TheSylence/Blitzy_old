// $Id$

using Blitzy.Plugin.System;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class PuttySettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void BrowseTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();

			PuttySettingsViewModel vm = baseVM.PuttySettings;

			Assert.IsTrue( vm.BrowsePuttyCommand.CanExecute( null ) );

			StringServiceMock mock = new StringServiceMock();
			DialogServiceManager.RegisterService( typeof( OpenFileService ), mock );
			mock.Value = null;

			vm.BrowsePuttyCommand.Execute( null );
			Assert.AreEqual( baseVM.Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey ), vm.PuttyPath );

			mock.Value = "test.exe";
			vm.BrowsePuttyCommand.Execute( null );
			Assert.AreEqual( "test.exe", vm.PuttyPath );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();

			PuttySettingsViewModel vm = baseVM.PuttySettings;
			vm.PuttyPath = null;

			PropertyChangedListener listener = new PropertyChangedListener( vm );
			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SaveTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();

			PuttySettingsViewModel vm = baseVM.PuttySettings;

			vm.PuttyPath = "testpath";
			vm.ImportSessions = !vm.ImportSessions;

			vm.Save();

			Assert.AreEqual( vm.PuttyPath, baseVM.Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey ) );
			Assert.AreEqual( vm.ImportSessions, baseVM.Settings.GetPluginSetting<bool>( Putty.GuidString, Putty.ImportKey ) );
		}
	}
}
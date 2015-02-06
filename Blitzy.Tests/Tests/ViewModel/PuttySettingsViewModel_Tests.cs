using Blitzy.Plugin.SystemPlugins;
using Blitzy.Tests.Mocks;
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
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( OpenFileService ), mock );

			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				PuttySettingsViewModel vm = baseVM.GetPluginContext<PuttySettingsViewModel>( "Putty" );

				Assert.IsTrue( vm.BrowsePuttyCommand.CanExecute( null ) );

				mock.Value = null;

				vm.BrowsePuttyCommand.Execute( null );
				Assert.AreEqual( baseVM.Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey ), vm.PuttyPath );

				mock.Value = "test.exe";
				vm.BrowsePuttyCommand.Execute( null );
				Assert.AreEqual( "test.exe", vm.PuttyPath );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory ) )
			{
				baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				PuttySettingsViewModel vm = baseVM.GetPluginContext<PuttySettingsViewModel>( "Putty" );
				vm.PuttyPath = null;

				PropertyChangedListener listener = new PropertyChangedListener( vm );
				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SaveTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory ) )
			{
				baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				PuttySettingsViewModel vm = baseVM.GetPluginContext<PuttySettingsViewModel>( "Putty" );

				vm.PuttyPath = "testpath";
				vm.ImportSessions = !vm.ImportSessions;

				vm.Save();

				Assert.AreEqual( vm.PuttyPath, baseVM.Settings.GetPluginSetting<string>( Putty.GuidString, Putty.PathKey ) );
				Assert.AreEqual( vm.ImportSessions, baseVM.Settings.GetPluginSetting<bool>( Putty.GuidString, Putty.ImportKey ) );
			}
		}
	}
}
// $Id$

using System.Linq;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebySettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void AddTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel() )
			{
				baseVM.Settings = new Blitzy.Model.Settings( Connection );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, Connection );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

				DataManipulationServiceMock<WebyWebsite> mock = new DataManipulationServiceMock<WebyWebsite>();
				DialogServiceManager.RegisterManipService( typeof( WebyWebsite ), mock );

				mock.CreateFunc = () => { return null; };

				Assert.IsTrue( vm.AddWebsiteCommand.CanExecute( null ) );

				Assert.AreEqual( 6, vm.Websites.Count );
				vm.AddWebsiteCommand.Execute( null );
				Assert.AreEqual( 6, vm.Websites.Count );

				mock.CreateFunc = () =>
				{
					return new WebyWebsite()
					{
						ID = 123,
						Name = "test",
						URL = "http://example.invalid",
						Description = "This is a test",
						Icon = ""
					};
				};

				vm.AddWebsiteCommand.Execute( null );
				Assert.AreEqual( 7, vm.Websites.Count );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EditTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel() )
			{
				baseVM.Settings = new Blitzy.Model.Settings( Connection );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, Connection );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

				vm.Websites.Add( new WebyWebsite()
				{
					ID = 123,
					Name = "test",
					URL = "http://example.invalid",
					Description = "This is a test",
					Icon = ""
				} );

				vm.Save();
				vm.Websites.First().Name = "example";
				vm.Save();

				baseVM.Reset();
				vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

				Assert.AreEqual( 7, vm.Websites.Count );
				Assert.AreEqual( "example", vm.Websites.First().Name );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel() )
			{
				baseVM.Settings = new Blitzy.Model.Settings( Connection );
				MockPluginHost host = new MockPluginHost( baseVM.Settings );
				baseVM.PluginManager = new Plugin.PluginManager( host, Connection );
				baseVM.PluginManager.LoadPlugins();
				baseVM.Reset();

				WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

				vm.Websites.Add( new WebyWebsite()
					{
						ID = 123,
						Name = "test",
						URL = "http://example.invalid",
						Description = "This is a test",
						Icon = ""
					} );

				Assert.IsFalse( vm.RemoveWebsiteCommand.CanExecute( null ) );
				vm.SelectedWebsite = vm.Websites.First();
				Assert.IsTrue( vm.RemoveWebsiteCommand.CanExecute( null ) );

				MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
				DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

				Assert.AreEqual( 7, vm.Websites.Count );
				vm.RemoveWebsiteCommand.Execute( null );
				Assert.AreEqual( 7, vm.Websites.Count );

				mock.Result = System.Windows.MessageBoxResult.Yes;

				vm.RemoveWebsiteCommand.Execute( null );
				Assert.AreEqual( 6, vm.Websites.Count );
				Assert.IsNull( vm.SelectedWebsite );

				vm.Save();

				baseVM.Reset();
				vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

				Assert.AreEqual( 6, vm.Websites.Count );
			}
		}
	}
}
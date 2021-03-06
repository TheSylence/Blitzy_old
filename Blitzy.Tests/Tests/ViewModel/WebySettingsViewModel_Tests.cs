﻿using System.Linq;
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
			DataManipulationServiceMock<WebyWebsite> mock = new DataManipulationServiceMock<WebyWebsite>();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterManipService( typeof( WebyWebsite ), mock );

			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( baseVM.Settings );
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.PluginManager.LoadPlugins();
						baseVM.Reset();

						WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

						mock.CreateFunc = () => { return null; };

						Assert.IsTrue( vm.AddWebsiteCommand.CanExecute( null ) );

						int oldCount = vm.Websites.Count;
						vm.AddWebsiteCommand.Execute( null );
						Assert.AreEqual( oldCount, vm.Websites.Count );

						WebyWebsite website = null;
						mock.CreateFunc = () =>
						{
							website = new WebyWebsite()
							{
								ID = TestHelper.NextID(),
								Name = "test",
								URL = "http://example.invalid",
								Description = "This is a test",
								Icon = ""
							};

							return website;
						};

						vm.AddWebsiteCommand.Execute( null );
						Assert.AreEqual( oldCount + 1, vm.Websites.Count );

						website.Dispose();
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EditTest()
		{
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( baseVM.Settings );
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.PluginManager.LoadPlugins();
						baseVM.Reset();

						WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

						using( WebyWebsite website = new WebyWebsite()
						{
							ID = TestHelper.NextID(),
							Name = "test",
							URL = "http://example.invalid",
							Description = "This is a test",
							Icon = ""
						} )
						{
							vm.Websites.Add( website );

							int count = vm.Websites.Count;

							vm.Save();
							vm.Websites.First().Name = "example";
							vm.Save();

							baseVM.Reset();
							vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

							Assert.AreEqual( count, vm.Websites.Count );
							Assert.AreEqual( "example", vm.Websites.First().Name );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( baseVM.Settings );
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.PluginManager.LoadPlugins();
						baseVM.Reset();

						WebySettingsViewModel vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

						using( WebyWebsite website =
						new WebyWebsite()
							{
								ID = TestHelper.NextID(),
								Name = "test",
								URL = "http://example.invalid",
								Description = "This is a test",
								Icon = ""
							} )
						{
							vm.Websites.Add( website );

							Assert.IsFalse( vm.RemoveWebsiteCommand.CanExecute( null ) );
							vm.SelectedWebsite = vm.Websites.First();
							Assert.IsTrue( vm.RemoveWebsiteCommand.CanExecute( null ) );

							int oldCount = vm.Websites.Count;
							vm.RemoveWebsiteCommand.Execute( null );
							Assert.AreEqual( oldCount, vm.Websites.Count );

							mock.Result = System.Windows.MessageBoxResult.Yes;

							vm.RemoveWebsiteCommand.Execute( null );
							Assert.AreEqual( oldCount - 1, vm.Websites.Count );
							Assert.IsNull( vm.SelectedWebsite );

							vm.Save();

							baseVM.Reset();
							vm = baseVM.GetPluginContext<WebySettingsViewModel>( "Weby" );

							Assert.AreEqual( oldCount - 1, vm.Websites.Count );
						}
					}
				}
			}
		}
	}
}
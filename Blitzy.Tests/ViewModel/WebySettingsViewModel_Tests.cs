// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebySettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void AddTest()
		{
			SettingsViewModel basevm = new SettingsViewModel();
			basevm.Settings = new Blitzy.Model.Settings( Connection );
			basevm.Reset();

			WebySettingsViewModel vm = basevm.WebySettings;

			DataManipulationServiceMock<WebyWebsite> mock = new DataManipulationServiceMock<WebyWebsite>();
			DialogServiceManager.RegisterManipService( typeof( WebyWebsite ), mock );

			mock.CreateFunc = () => { return null; };

			Assert.IsTrue( vm.AddWebsiteCommand.CanExecute( null ) );

			Assert.AreEqual( 0, vm.Websites.Count );
			vm.AddWebsiteCommand.Execute( null );
			Assert.AreEqual( 0, vm.Websites.Count );

			mock.CreateFunc = () =>
			{
				return new WebyWebsite()
				{
					ID = 123,
					Name = "test",
					URL = "http://example.invalid",
					Description = "This is a test"
				};
			};

			vm.AddWebsiteCommand.Execute( null );
			Assert.AreEqual( 1, vm.Websites.Count );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EditTest()
		{
			SettingsViewModel basevm = new SettingsViewModel();
			basevm.Settings = new Blitzy.Model.Settings( Connection );
			basevm.Reset();

			WebySettingsViewModel vm = basevm.WebySettings;

			vm.Websites.Add( new WebyWebsite()
			{
				ID = 123,
				Name = "test",
				URL = "http://example.invalid",
				Description = "This is a test"
			} );

			vm.Save();
			vm.Websites.First().Name = "example";
			vm.Save();

			basevm.Reset();
			vm = basevm.WebySettings;

			Assert.AreEqual( 1, vm.Websites.Count );
			Assert.AreEqual( "example", vm.Websites.First().Name );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveTest()
		{
			SettingsViewModel basevm = new SettingsViewModel();
			basevm.Settings = new Blitzy.Model.Settings( Connection );
			basevm.Reset();

			WebySettingsViewModel vm = basevm.WebySettings;

			vm.Websites.Add( new WebyWebsite()
				{
					ID = 123,
					Name = "test",
					URL = "http://example.invalid",
					Description = "This is a test"
				} );

			Assert.IsFalse( vm.RemoveWebsiteCommand.CanExecute( null ) );
			vm.SelectedWebsite = vm.Websites.First();
			Assert.IsTrue( vm.RemoveWebsiteCommand.CanExecute( null ) );

			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			Assert.AreEqual( 1, vm.Websites.Count );
			vm.RemoveWebsiteCommand.Execute( null );
			Assert.AreEqual( 1, vm.Websites.Count );

			mock.Result = System.Windows.MessageBoxResult.Yes;

			vm.RemoveWebsiteCommand.Execute( null );
			Assert.AreEqual( 0, vm.Websites.Count );
			Assert.IsNull( vm.SelectedWebsite );

			vm.Save();

			basevm.Reset();
			vm = basevm.WebySettings;

			Assert.AreEqual( 0, vm.Websites.Count );
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Tests.Mocks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class PluginsDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void CommandsTest()
		{
			using( PluginsDialogViewModel vm = new PluginsDialogViewModel() )
			{
				Assert.IsFalse( vm.DisableCommand.CanExecute( null ) );
				Assert.IsFalse( vm.EnableCommand.CanExecute( null ) );
				Assert.IsTrue( vm.InstallCommand.CanExecute( null ) );

				vm.SelectedPlugin = new PluginInformation() { Enabled = true };

				Assert.IsTrue( vm.DisableCommand.CanExecute( null ) );
				Assert.IsFalse( vm.EnableCommand.CanExecute( null ) );

				vm.SelectedPlugin = new PluginInformation() { Enabled = false };

				Assert.IsFalse( vm.DisableCommand.CanExecute( null ) );
				Assert.IsTrue( vm.EnableCommand.CanExecute( null ) );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EnableDisableTest()
		{
			MockPluginHost host = new MockPluginHost();

			using( PluginsDialogViewModel vm = new PluginsDialogViewModel() )
			{
				vm.PluginManager = new Plugin.PluginManager( host, Connection );
				vm.PluginManager.LoadPlugins();
				vm.Reset();

				vm.SelectedPlugin = vm.Plugins.First();

				Assert.IsTrue( vm.DisableCommand.CanExecute( null ) );
				vm.DisableCommand.Execute( null );

				Assert.IsFalse( vm.SelectedPlugin.Enabled );
				Assert.IsTrue( vm.PluginManager.DisabledPlugins.Contains( vm.SelectedPlugin.Instance ) );
				Assert.IsFalse( vm.PluginManager.Plugins.Contains( vm.SelectedPlugin.Instance ) );

				Assert.IsTrue( vm.EnableCommand.CanExecute( null ) );
				vm.EnableCommand.Execute( null );
				Assert.IsTrue( vm.SelectedPlugin.Enabled );
				Assert.IsFalse( vm.PluginManager.DisabledPlugins.Contains( vm.SelectedPlugin.Instance ) );
				Assert.IsTrue( vm.PluginManager.Plugins.Contains( vm.SelectedPlugin.Instance ) );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void InstallTest()
		{
			DelegateServiceMock mock = new DelegateServiceMock();
			mock.Action = ( param ) => null;
			DialogServiceManager.RegisterService( typeof( OpenFileService ), mock );

			using( PluginsDialogViewModel vm = new PluginsDialogViewModel() )
			{
				using( ShimsContext.Create() )
				{
					ProcessStartInfo startInfo = null;
					bool waitCalled = false;

					System.Diagnostics.Fakes.ShimProcess.AllInstances.Start = ( proc ) =>
						{
							startInfo = proc.StartInfo;
							return true;
						};

					System.Diagnostics.Fakes.ShimProcess.AllInstances.WaitForExit = ( proc ) =>
						{
							waitCalled = true;
						};

					vm.InstallCommand.Execute( null );
					Assert.IsNull( startInfo );
					Assert.IsFalse( waitCalled );

					File.WriteAllText( "test.zip", "test" );
					mock.Action = ( param ) => "test.zip";
					vm.InstallCommand.Execute( null );
					File.Delete( "test.zip" );

					Assert.IsNotNull( startInfo );
					Assert.AreEqual( "runas", startInfo.Verb );
					Assert.AreEqual( string.Format( "{0} \"{1}\"", Constants.CommandLine.InstallPlugin, "test.zip" ), startInfo.Arguments );
					Assert.IsTrue( waitCalled );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( PluginsDialogViewModel vm = new PluginsDialogViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );
				Assert.IsTrue( listener.TestProperties() );
			}
		}
	}
}
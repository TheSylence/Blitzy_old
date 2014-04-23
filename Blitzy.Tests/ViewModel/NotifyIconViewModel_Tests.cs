// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class NotifyIconViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void IconTest()
		{
			NotifyIconViewModel vm = new NotifyIconViewModel();
			vm.MainVM = new MainViewModel();
			vm.Reset();

			Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Finished, null, null ) );
			Assert.IsTrue( vm.IconSource.Contains( "TrayIcon.ico" ) );

			Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Error, null, null ) );
			Assert.IsTrue( vm.IconSource.Contains( "TrayIconFailure.ico" ) );

			Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Executing, null, null ) );
			Assert.IsTrue( vm.IconSource.Contains( "CommandExecuting.ico" ) );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			NotifyIconViewModel obj = new NotifyIconViewModel();
			PropertyChangedListener listener = new PropertyChangedListener( obj );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void QuitTest()
		{
			NotifyIconViewModel vm = new NotifyIconViewModel();
			vm.MainVM = new MainViewModel();

			bool quit = false;
			Messenger.Default.Register<InternalCommandMessage>( this, msg => quit = msg.Command.Equals( "quit" ) );

			Assert.IsTrue( vm.QuitCommand.CanExecute( null ) );
			vm.QuitCommand.Execute( null );
			Assert.IsTrue( quit );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SettingsTest()
		{
			NotifyIconViewModel vm = new NotifyIconViewModel();
			vm.MainVM = new MainViewModel();

			CallCheckServiceMock mock = new CallCheckServiceMock();
			DialogServiceManager.RegisterService( typeof( SettingsService ), mock );

			Assert.IsTrue( vm.SettingsCommand.CanExecute( null ) );
			vm.SettingsCommand.Execute( null );

			Assert.IsTrue( mock.WasCalled );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ShowTest()
		{
			NotifyIconViewModel vm = new NotifyIconViewModel();
			vm.MainVM = new MainViewModel();
			bool shown = false;
			vm.MainVM.RequestShow += ( s, e ) => shown = true;

			Assert.IsTrue( vm.ShowCommand.CanExecute( null ) );
			vm.ShowCommand.Execute( null );

			Assert.IsTrue( shown );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void VisibleTest()
		{
			NotifyIconViewModel vm = new NotifyIconViewModel();
			vm.MainVM = new MainViewModel();
			vm.MainVM.Settings.SetDefaults();

			Assert.IsTrue( vm.Visible );
			vm.MainVM.Settings.SetValue( Blitzy.Model.SystemSetting.TrayIcon, false );
			Assert.IsFalse( vm.Visible );
		}
	}
}
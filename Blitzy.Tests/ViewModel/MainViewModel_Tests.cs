// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Blitzy.Messages;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class MainViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void QuitCommandTest()
		{
			bool? closed = null;
			MainViewModel vm = new MainViewModel();
			vm.Reset();

			vm.RequestClose += ( s, e ) =>
				{
					closed = e.Result;
				};
			vm.CommandInput = "quit";

			Assert.IsNotNull( vm.CmdManager.CurrentItem );

			bool started = false;
			bool completed = false;

			Messenger.Default.Register<CommandMessage>( this, msg =>
			{
				if( msg.Status == CommandStatus.Executing )
				{
					started = true;
				}
				else if( msg.Status == CommandStatus.Finished || msg.Status == CommandStatus.Error )
				{
					completed = true;
				}
			} );

			Assert.IsTrue( vm.ExecuteCommand.CanExecute( null ) );
			vm.ExecuteCommand.Execute( null );

			Assert.IsTrue( started );
			Assert.IsTrue( completed );

			Assert.AreEqual( true, closed );
		}

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
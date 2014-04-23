// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
		public void ArrowsTest()
		{
			MainViewModel vm = new MainViewModel();
			vm.CommandInput = "t";

			Assert.IsTrue( vm.CmdManager.Items.Count > 1 );

			Assert.AreEqual( 0, vm.SelectedCommandIndex );
			Assert.IsTrue( vm.OnKeyDownArrow() );
			Assert.AreEqual( 1, vm.SelectedCommandIndex );

			Assert.IsTrue( vm.OnKeyUpArrow() );
			Assert.AreEqual( 0, vm.SelectedCommandIndex );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void BackTest()
		{
			MainViewModel vm = new MainViewModel();
			vm.CommandInput = "test";
			Assert.IsFalse( vm.OnKeyBack() );
			Assert.AreEqual( "test", vm.CommandInput );

			vm.CommandInput = "test" + vm.CmdManager.Separator;
			Assert.IsTrue( vm.OnKeyBack() );
			Assert.AreEqual( "test", vm.CommandInput );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CommandsTest()
		{
			MainViewModel vm = new MainViewModel();
			Assert.IsTrue( vm.OnClosingCommand.CanExecute( null ) );
			Assert.IsTrue( vm.OnDeactivatedCommand.CanExecute( null ) );
			Assert.IsTrue( vm.KeyPreviewCommand.CanExecute( null ) );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeactivatedTest()
		{
			MainViewModel vm = new MainViewModel();
			bool hidden = false;
			vm.RequestHide += ( s, e ) => hidden = true;

			vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnFocusLost, false );
			Assert.IsTrue( vm.OnDeactivatedCommand.CanExecute( null ) );
			vm.OnDeactivatedCommand.Execute( null );
			Assert.IsFalse( hidden );

			vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnFocusLost, true );
			Assert.IsTrue( vm.OnDeactivatedCommand.CanExecute( null ) );
			vm.OnDeactivatedCommand.Execute( null );
			Assert.IsTrue( hidden );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EscapeTest()
		{
			MainViewModel vm = new MainViewModel();
			bool hidden = false;
			vm.RequestHide += ( s, e ) => hidden = true;

			vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnEscape, false );
			Assert.IsFalse( vm.OnKeyEscape() );
			Assert.IsFalse( hidden );

			vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnEscape, true );
			Assert.IsTrue( vm.OnKeyEscape() );
			Assert.IsTrue( hidden );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			MainViewModel vm = new MainViewModel();
			PropertyChangedListener listener = new PropertyChangedListener( vm );
			listener.Exclude<MainViewModel>( o => o.ShouldClose );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void QuitCommandTest()
		{
			bool? closed = null;
			MainViewModel vm = new MainViewModel();
			vm.Reset();
			Assert.IsFalse( vm.ExecuteCommand.CanExecute( null ) );

			vm.RequestClose += ( s, e ) =>
				{
					closed = e.Result;
				};
			vm.CommandInput = "quit";

			Assert.IsTrue( vm.ExecuteCommand.CanExecute( null ) );
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
		public void ReturnTest()
		{
			MainViewModel vm = new MainViewModel();
			Assert.IsFalse( vm.OnKeyReturn() );

			vm.CommandInput = "quit";

			bool started = false;
			Messenger.Default.Register<CommandMessage>( this, msg =>
			{
				if( msg.Status == CommandStatus.Executing )
				{
					started = true;
				}
			} );

			Assert.IsTrue( vm.OnKeyReturn() );
			Assert.IsTrue( started );
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

		[TestMethod, TestCategory( "ViewModel" )]
		public void TabTest()
		{
			MainViewModel vm = new MainViewModel();
			Assert.IsFalse( vm.OnKeyTab() );

			vm.CommandInput = "qui";
			Assert.IsTrue( vm.OnKeyTab() );
			Assert.AreEqual( "quit", vm.CommandInput );
			Assert.IsNotNull( vm.CmdManager.CurrentItem );
			Assert.AreEqual( "quit", vm.CmdManager.CurrentItem.Name );

			vm.CommandInput = "medy";
			Assert.IsTrue( vm.OnKeyTab() );
			Assert.AreEqual( "medy" + vm.CmdManager.Separator, vm.CommandInput );
			Assert.IsNotNull( vm.CmdManager.CurrentItem );
			Assert.AreNotEqual( "medy", vm.CmdManager.CurrentItem.Name );
			Assert.IsNotNull( vm.CmdManager.CurrentItem.Parent );
			Assert.AreEqual( "medy", vm.CmdManager.CurrentItem.Parent.Name );

			vm.CommandInput = "med";
			Assert.IsTrue( vm.OnKeyTab() );
			Assert.AreEqual( "medy" + vm.CmdManager.Separator, vm.CommandInput );
			Assert.IsNotNull( vm.CmdManager.CurrentItem );
			Assert.AreNotEqual( "medy", vm.CmdManager.CurrentItem.Name );
			Assert.IsNotNull( vm.CmdManager.CurrentItem.Parent );
			Assert.AreEqual( "medy", vm.CmdManager.CurrentItem.Parent.Name );

			vm.CommandInput += "play";
			Assert.IsTrue( vm.OnKeyTab() );
			Assert.AreEqual( "medy" + vm.CmdManager.Separator + "play", vm.CommandInput );
			Assert.IsNotNull( vm.CmdManager.CurrentItem );
			Assert.AreEqual( "play", vm.CmdManager.CurrentItem.Name );
		}
	}
}
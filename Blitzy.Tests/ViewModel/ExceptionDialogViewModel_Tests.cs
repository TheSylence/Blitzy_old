﻿// $Id$

using System;
using System.Diagnostics;
using System.Windows;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ExceptionDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void ExitTest()
		{
			ExceptionDialogViewModel vm = new ExceptionDialogViewModel( new Exception(), new StackTrace( true ) );

			bool closed = false;
			vm.RequestClose += ( s, e ) => closed = true;

			Assert.IsTrue( vm.ExitCommand.CanExecute( null ) );
			vm.ExitCommand.Execute( null );
			Assert.IsTrue( closed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SendTest()
		{
			Exception ex = new Exception( "This is a test" );
			ExceptionDialogViewModel vm = new ExceptionDialogViewModel( ex, new StackTrace( true ) );

			bool closed = false;
			vm.RequestClose += ( s, e ) => closed = true;

			bool success = false;
			bool called = false;
			DelegateServiceMock mock = new DelegateServiceMock();
			mock.Action = ( args ) =>
				{
					called = true;
					success = ( (MessageBoxParameter)args ).Icon == MessageBoxImage.Information;
					return null;
				};

			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			Assert.IsTrue( vm.SendCommand.CanExecute( null ) );
			vm.SendCommand.Execute( null );
			Assert.IsTrue( called );
			Assert.IsTrue( success );
			Assert.IsTrue( closed );
		}
	}
}
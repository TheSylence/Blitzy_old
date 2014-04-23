// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewModel.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ExceptionDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void ExitTest()
		{
			ExceptionDialogViewModel vm = new ExceptionDialogViewModel( new Exception() );

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
			ExceptionDialogViewModel vm = new ExceptionDialogViewModel( ex );

			bool closed = false;
			vm.RequestClose += ( s, e ) => closed = true;

			Assert.IsTrue( vm.SendCommand.CanExecute( null ) );
			vm.SendCommand.Execute( null );
			Assert.IsTrue( closed );

			// TODO: Test if error report was sent
		}
	}
}
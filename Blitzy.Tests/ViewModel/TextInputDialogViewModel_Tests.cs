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
	[TestClass]
	public class TextInputDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void OkTest()
		{
			bool? closed = null;
			TextInputDialogViewModel vm = new TextInputDialogViewModel();
			vm.RequestClose += ( s, e ) => closed = e.Result;

			Assert.IsTrue( vm.OkCommand.CanExecute( null ) );
			Assert.IsTrue( vm.CancelCommand.CanExecute( null ) );

			vm.OkCommand.Execute( null );
			Assert.AreEqual( true, closed );

			vm.CancelCommand.Execute( null );
			Assert.AreEqual( false, closed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			TextInputDialogViewModel obj = new TextInputDialogViewModel();
			PropertyChangedListener listener = new PropertyChangedListener( obj );

			Assert.IsTrue( listener.TestProperties() );
		}
	}
}
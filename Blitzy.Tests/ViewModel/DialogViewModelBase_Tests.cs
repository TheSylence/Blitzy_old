// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Tests.Mocks;
using Blitzy.ViewModel.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class DialogViewModelBase_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void CloseTest()
		{
			bool? closed = null;
			DialogVM vm = new DialogVM();
			vm.RequestClose += ( s, e ) => closed = e.Result;

			Assert.IsTrue( vm.OkCommand.CanExecute( null ) );
			Assert.IsTrue( vm.CancelCommand.CanExecute( null ) );

			vm.OkCommand.Execute( null );
			Assert.AreEqual( true, closed );
			vm.CancelCommand.Execute( null );
			Assert.AreEqual( false, closed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void NewTest()
		{
			DialogVM vm = new DialogVM();
			vm.New = true;
			Assert.AreEqual( "Add".Localize(), vm.Title );
			vm.New = false;
			Assert.AreEqual( "Edit".Localize(), vm.Title );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			DialogVM obj = new DialogVM();
			PropertyChangedListener listener = new PropertyChangedListener( obj );
			//listener.Exclude<DialogVM>( o => o.ID );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ResetTest()
		{
			DialogVM vm = new DialogVM();
			Assert.IsNull( vm.Model );
			vm.Reset();
			Assert.IsNotNull( vm.Model );
		}

		private class DialogVM : DialogViewModelBase<MockModel>
		{
		}
	}
}
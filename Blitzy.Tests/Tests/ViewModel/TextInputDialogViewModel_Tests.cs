

using Blitzy.ViewModel.Dialogs;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class TextInputDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void OkTest()
		{
			bool? closed = null;
			using( TextInputDialogViewModel vm = new TextInputDialogViewModel() )
			{
				vm.RequestClose += ( s, e ) => closed = e.Result;

				Assert.IsTrue( vm.OkCommand.CanExecute( null ) );
				Assert.IsTrue( vm.CancelCommand.CanExecute( null ) );

				vm.OkCommand.Execute( null );
				Assert.AreEqual( true, closed );

				vm.CancelCommand.Execute( null );
				Assert.AreEqual( false, closed );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( TextInputDialogViewModel obj = new TextInputDialogViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( obj );

				Assert.IsTrue( listener.TestProperties() );
			}
		}
	}
}
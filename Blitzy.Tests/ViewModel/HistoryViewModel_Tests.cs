using Blitzy.Messages;
using Blitzy.ViewModel;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class HistoryViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void MessageTest()
		{
			Messenger messenger = new Messenger();
			using( HistoryViewModel vm = new HistoryViewModel( messenger ) )
			{
				vm.Manager = new Blitzy.Model.HistoryManager( new Blitzy.Model.Settings( Connection ) );
				vm.Manager.AddItem( "item1" );
				vm.Manager.AddItem( "item2" );
				vm.Manager.AddItem( "item3" );

				Assert.IsNull( vm.Manager.SelectedItem );

				messenger.Send<HistoryMessage>( new HistoryMessage( HistoryMessageType.Down ) );
				Assert.AreEqual( "item2", vm.Manager.SelectedItem );
				messenger.Send<HistoryMessage>( new HistoryMessage( HistoryMessageType.Down ) );
				Assert.AreEqual( "item3", vm.Manager.SelectedItem );

				messenger.Send<HistoryMessage>( new HistoryMessage( HistoryMessageType.Down ) );
				Assert.AreEqual( "item1", vm.Manager.SelectedItem );
				messenger.Send<HistoryMessage>( new HistoryMessage( HistoryMessageType.Up ) );
				Assert.AreEqual( "item3", vm.Manager.SelectedItem );
			}
		}
	}
}
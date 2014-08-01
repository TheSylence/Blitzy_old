// $Id$

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class MainViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void ArrowsTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.CommandInput = "t";

				Assert.IsTrue( vm.CmdManager.Items.Count > 1 );

				Assert.AreEqual( 0, vm.SelectedCommandIndex );
				Assert.IsTrue( vm.OnKeyDownArrow() );
				Assert.AreEqual( 1, vm.SelectedCommandIndex );

				Assert.IsTrue( vm.OnKeyUpArrow() );
				Assert.AreEqual( 0, vm.SelectedCommandIndex );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void BackTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.CommandInput = "test";
				Assert.IsFalse( vm.OnKeyBack() );
				Assert.AreEqual( "test", vm.CommandInput );

				vm.CommandInput = "test" + vm.CmdManager.Separator;
				Assert.IsTrue( vm.OnKeyBack() );
				Assert.AreEqual( "test", vm.CommandInput );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CommandsTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				Assert.IsTrue( vm.OnClosingCommand.CanExecute( null ) );
				Assert.IsTrue( vm.OnDeactivatedCommand.CanExecute( null ) );
				Assert.IsTrue( vm.KeyPreviewCommand.CanExecute( null ) );
				Assert.IsTrue( vm.KeyUpCommand.CanExecute( null ) );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeactivatedTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
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
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownTestHistory()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.History.Commands = new ObservableCollection<string>( new[] { "test", "test2" } );
				vm.History.SelectedItem = "historytest";
				Messenger.Default.Unregister( vm.History );
				bool receivedHistoryShow = false;
				bool receivedHistoryDown = false;

				Messenger.Default.Register<HistoryMessage>( this, ( msg ) =>
				{
					if( msg.Type == HistoryMessageType.Show )
					{
						receivedHistoryShow = true;
						Assert.IsNotNull( msg.History );
					}
					else if( msg.Type == HistoryMessageType.Down )
					{
						receivedHistoryDown = true;
					}
				} );

				using( ShimsContext.Create() )
				{
					HashSet<Key> pressedKeys = new HashSet<Key>();
					pressedKeys.Add( Key.LeftCtrl );

					System.Windows.Input.Fakes.ShimKeyboard.IsKeyDownKey = ( k ) =>
					{
						return pressedKeys.Contains( k );
					};

					Assert.IsTrue( vm.OnKeyDownArrow() );
					Assert.IsTrue( receivedHistoryDown );
					Assert.IsTrue( receivedHistoryShow );

					receivedHistoryShow = false;
					receivedHistoryDown = false;

					pressedKeys.Clear();
					pressedKeys.Add( Key.RightCtrl );

					Assert.IsTrue( vm.OnKeyDownArrow() );
					Assert.IsTrue( receivedHistoryDown );
					Assert.IsTrue( receivedHistoryShow );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void EscapeTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				bool hidden = false;
				vm.RequestHide += ( s, e ) => hidden = true;

				vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnEscape, false );
				Assert.IsFalse( vm.OnKeyEscape() );
				Assert.IsFalse( hidden );

				vm.Settings.SetValue( Blitzy.Model.SystemSetting.CloseOnEscape, true );
				Assert.IsTrue( vm.OnKeyEscape() );
				Assert.IsTrue( hidden );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void InternalCommandHistoryTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.History.AddItem( "item1" );
				vm.History.AddItem( "item2" );
				vm.History.AddItem( "item3" );
				Assert.AreEqual( 3, vm.History.Commands.Count );

				Messenger.Default.Send( new InternalCommandMessage( "history" ) );

				Assert.AreEqual( 0, vm.History.Commands.Count );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void MouseCommandTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.Reset();
				Assert.IsFalse( vm.ExecuteCommand.CanExecute( null ) );
				Assert.IsFalse( vm.MouseExecuteCommand.CanExecute( null ) );

				vm.CommandInput = "quit";

				Assert.IsTrue( vm.ExecuteCommand.CanExecute( null ) );
				Assert.IsTrue( vm.MouseExecuteCommand.CanExecute( null ) );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );
				listener.Exclude<MainViewModel>( o => o.ShouldClose );

				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void QuitCommandTest()
		{
			bool? closed = null;
			using( MainViewModel vm = new MainViewModel() )
			{
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
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ReturnHistoryTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.History.SelectedItem = "historytest";
				bool receivedHistoryClose = false;
				bool receivedNewCaret = false;

				Messenger.Default.Register<HistoryMessage>( this, ( msg ) => receivedHistoryClose = msg.Type == HistoryMessageType.Hide );
				Messenger.Default.Register<InputCaretPositionMessage>( this, ( msg ) => receivedNewCaret = true );

				using( ShimsContext.Create() )
				{
					HashSet<Key> pressedKeys = new HashSet<Key>();
					pressedKeys.Add( Key.LeftCtrl );

					System.Windows.Input.Fakes.ShimKeyboard.IsKeyDownKey = pressedKeys.Contains;

					Assert.IsTrue( vm.OnKeyReturn() );
					Assert.AreEqual( "historytest", vm.CommandInput );
					Assert.IsTrue( receivedNewCaret );
					Assert.IsTrue( receivedHistoryClose );

					receivedHistoryClose = false;
					receivedNewCaret = false;

					pressedKeys.Clear();
					pressedKeys.Add( Key.RightCtrl );

					Assert.IsTrue( vm.OnKeyReturn() );
					Assert.AreEqual( "historytest", vm.CommandInput );
					Assert.IsTrue( receivedNewCaret );
					Assert.IsTrue( receivedHistoryClose );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ReturnSecondaryTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				MockPlugin plug = new MockPlugin();
				CommandItem item = CommandItem.Create( "test", "test", plug );

				using( ShimsContext.Create() )
				{
					HashSet<Key> pressedKeys = new HashSet<Key>();
					pressedKeys.Add( Key.LeftCtrl );
					pressedKeys.Add( Key.LeftShift );

					System.Windows.Input.Fakes.ShimKeyboard.IsKeyDownKey = pressedKeys.Contains;

					vm.CommandInput = "test";
					vm.CmdManager.CurrentItem = item;
					Assert.IsTrue( vm.OnKeyReturn() );

					pressedKeys.Clear();
					pressedKeys.Add( Key.RightCtrl );
					pressedKeys.Add( Key.RightShift );

					vm.CommandInput = "test";
					vm.CmdManager.CurrentItem = item;
					Assert.IsTrue( vm.OnKeyReturn() );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ReturnTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
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
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SettingsTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				Assert.IsTrue( vm.SettingsCommand.CanExecute( null ) );

				CallCheckServiceMock mock = new CallCheckServiceMock();
				DialogServiceManager.RegisterService( typeof( SettingsService ), mock );

				vm.SettingsCommand.Execute( null );

				Assert.IsTrue( mock.WasCalled );
				Assert.IsNotNull( mock.Parameter );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void TabTest()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
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

				vm.CommandInput = "calcy";
				Assert.IsTrue( vm.OnKeyTab() );
				Assert.IsNotNull( vm.CmdManager.CurrentItem );

				vm.CommandInput += "3423423";
				Assert.IsTrue( vm.OnKeyTab() );
				Assert.AreEqual( "calcy" + vm.CmdManager.Separator + "3423423", vm.CommandInput );
				Assert.IsNotNull( vm.CmdManager.CurrentItem );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void UpTestHistory()
		{
			using( MainViewModel vm = new MainViewModel() )
			{
				vm.History.Commands = new ObservableCollection<string>( new[] { "test", "test2" } );
				vm.History.SelectedItem = "historytest";

				Messenger.Default.Unregister( vm.History );
				bool receivedHistoryShow = false;
				bool receivedHistoryUp = false;

				Messenger.Default.Register<HistoryMessage>( this, ( msg ) =>
					{
						if( msg.Type == HistoryMessageType.Show )
						{
							receivedHistoryShow = true;
							Assert.IsNotNull( msg.History );
						}
						else if( msg.Type == HistoryMessageType.Up )
						{
							receivedHistoryUp = true;
						}
					} );

				using( ShimsContext.Create() )
				{
					HashSet<Key> pressedKeys = new HashSet<Key>();
					pressedKeys.Add( Key.LeftCtrl );

					System.Windows.Input.Fakes.ShimKeyboard.IsKeyDownKey = ( k ) =>
					{
						return pressedKeys.Contains( k );
					};

					Assert.IsTrue( vm.OnKeyUpArrow() );
					Assert.IsTrue( receivedHistoryUp );
					Assert.IsTrue( receivedHistoryShow );

					receivedHistoryShow = false;
					receivedHistoryUp = false;

					pressedKeys.Clear();
					pressedKeys.Add( Key.RightCtrl );

					Assert.IsTrue( vm.OnKeyUpArrow() );
					Assert.IsTrue( receivedHistoryUp );
					Assert.IsTrue( receivedHistoryShow );
				}
			}
		}
	}
}
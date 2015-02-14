using System.Linq;
using Blitzy.Model;
using Blitzy.Tests.Mocks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WorkspaceSettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void AddItemTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( OpenFileService ), mock );

			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							Assert.IsFalse( vm.AddItemCommand.CanExecute( null ) );

							using( vm.SelectedWorkspace = new Blitzy.Model.Workspace() )
							{
								vm.Workspaces.Add( vm.SelectedWorkspace );

								Assert.IsTrue( vm.AddItemCommand.CanExecute( null ) );

								mock.Value = null;

								vm.AddItemCommand.Execute( null );
								Assert.AreEqual( 0, vm.SelectedWorkspace.Items.Count );

								mock.Value = "test";
								vm.AddItemCommand.Execute( null );
								Assert.AreEqual( 1, vm.SelectedWorkspace.Items.Count );

								mock.Value = "test2";
								vm.AddItemCommand.Execute( null );
								Assert.AreEqual( 2, vm.SelectedWorkspace.Items.Count );
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddWorkspaceTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );

			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							Assert.IsTrue( vm.AddWorkspaceCommand.CanExecute( null ) );
							int oldCount = vm.Workspaces.Count;

							mock.Value = null;
							vm.AddWorkspaceCommand.Execute( null );
							Assert.AreEqual( oldCount, vm.Workspaces.Count );

							mock.Value = "test";
							vm.AddWorkspaceCommand.Execute( null );
							Assert.AreEqual( oldCount + 1, vm.Workspaces.Count );

							mock.Value = "test2";
							vm.AddWorkspaceCommand.Execute( null );
							Assert.AreEqual( oldCount + 2, vm.Workspaces.Count );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeleteItemTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							Assert.IsFalse( vm.RemoveItemCommand.CanExecute( null ) );

							using( vm.SelectedWorkspace = new Blitzy.Model.Workspace() )
							{
								vm.Workspaces.Add( vm.SelectedWorkspace );

								Assert.IsFalse( vm.RemoveItemCommand.CanExecute( null ) );

								using( vm.SelectedItem = new Blitzy.Model.WorkspaceItem() )
								{
									vm.SelectedWorkspace.Items.Add( vm.SelectedItem );

									Assert.IsTrue( vm.RemoveItemCommand.CanExecute( null ) );

									mock.Result = System.Windows.MessageBoxResult.No;

									vm.RemoveItemCommand.Execute( null );
									Assert.AreEqual( 1, vm.SelectedWorkspace.Items.Count );

									mock.Result = System.Windows.MessageBoxResult.Yes;
									vm.RemoveItemCommand.Execute( null );
									Assert.AreEqual( 0, vm.SelectedWorkspace.Items.Count );
								}
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeleteWorkspaceTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							Assert.IsFalse( vm.DeleteWorkspaceCommand.CanExecute( null ) );

							using( vm.SelectedWorkspace = new Blitzy.Model.Workspace() )
							{
								vm.Workspaces.Add( vm.SelectedWorkspace );
								Assert.IsTrue( vm.DeleteWorkspaceCommand.CanExecute( null ) );

								mock.Result = System.Windows.MessageBoxResult.No;

								int oldCount = vm.Workspaces.Count;
								vm.DeleteWorkspaceCommand.Execute( null );
								Assert.IsNotNull( vm.SelectedWorkspace );
								Assert.AreEqual( oldCount, vm.Workspaces.Count );

								mock.Result = System.Windows.MessageBoxResult.Yes;
								vm.DeleteWorkspaceCommand.Execute( null );
								Assert.IsNull( vm.SelectedWorkspace );
								Assert.AreEqual( oldCount - 1, vm.Workspaces.Count );
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void MoveItemTest()
		{
			ViewServiceManager serviceManager = new ViewServiceManager();
			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							Assert.IsFalse( vm.MoveItemDownCommand.CanExecute( null ) );
							Assert.IsFalse( vm.MoveItemUpCommand.CanExecute( null ) );

							using( vm.SelectedWorkspace = new Blitzy.Model.Workspace() )
							{
								vm.Workspaces.Add( vm.SelectedWorkspace );

								Assert.IsFalse( vm.MoveItemDownCommand.CanExecute( null ) );
								Assert.IsFalse( vm.MoveItemUpCommand.CanExecute( null ) );

								using( WorkspaceItem item1 = new Blitzy.Model.WorkspaceItem() { ItemID = 1 } )
								using( WorkspaceItem item2 = new Blitzy.Model.WorkspaceItem() { ItemID = 2 } )
								using( WorkspaceItem item3 = new Blitzy.Model.WorkspaceItem() { ItemID = 3 } )
								{
									vm.SelectedWorkspace.Items.Add( item1 );
									vm.SelectedWorkspace.Items.Add( item2 );
									vm.SelectedWorkspace.Items.Add( item3 );

									vm.SelectedItem = vm.SelectedWorkspace.Items.First();
									Assert.IsTrue( vm.MoveItemDownCommand.CanExecute( null ) );
									Assert.IsFalse( vm.MoveItemUpCommand.CanExecute( null ) );

									vm.SelectedItem = vm.SelectedWorkspace.Items.Last();
									Assert.IsFalse( vm.MoveItemDownCommand.CanExecute( null ) );
									Assert.IsTrue( vm.MoveItemUpCommand.CanExecute( null ) );

									vm.SelectedItem = vm.SelectedWorkspace.Items[1];
									Assert.IsTrue( vm.MoveItemDownCommand.CanExecute( null ) );
									Assert.IsTrue( vm.MoveItemUpCommand.CanExecute( null ) );

									// 1,3,2
									vm.MoveItemDownCommand.Execute( null );

									vm.SelectedItem = vm.SelectedWorkspace.Items.First();
									// 3,1,2
									vm.MoveItemDownCommand.Execute( null );

									vm.SelectedItem = vm.SelectedWorkspace.Items.Last();
									// 3,2,1
									vm.MoveItemUpCommand.Execute( null );

									Assert.AreEqual( 1, vm.SelectedWorkspace.Items[0].ItemOrder );
									Assert.AreEqual( 2, vm.SelectedWorkspace.Items[1].ItemOrder );
									Assert.AreEqual( 3, vm.SelectedWorkspace.Items[2].ItemOrder );

									Assert.AreEqual( 3, vm.SelectedWorkspace.Items[0].ItemID );
									Assert.AreEqual( 2, vm.SelectedWorkspace.Items[1].ItemID );
									Assert.AreEqual( 1, vm.SelectedWorkspace.Items[2].ItemID );
								}
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			ViewServiceManager serviceManager = new ViewServiceManager();
			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							PropertyChangedListener listener = new PropertyChangedListener( vm );
							Assert.IsTrue( listener.TestProperties() );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SaveLoadTest()
		{
			int expectedCount;
			int id = TestHelper.NextID();

			ViewServiceManager serviceManager = new ViewServiceManager();
			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							using( Workspace ws = new Blitzy.Model.Workspace
							{
								ID = id,
								Name = "Test"
							} )
							{
								vm.Workspaces.Add( ws );

								expectedCount = vm.Workspaces.Count;

								vm.Save();
							}
						}
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							vm.Reset();

							Assert.AreEqual( expectedCount, vm.Workspaces.Count );
							Assert.AreEqual( "Test", vm.Workspaces.Last().Name );
							Assert.AreEqual( id, vm.Workspaces.Last().ID );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void UniqueConstraintFail()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );
			serviceManager.RegisterService( typeof( OpenFileService ), mock );

			MockPluginHost host = new MockPluginHost();
			using( SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						baseVM.Reset();
						using( WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( ConnectionFactory, baseVM.Settings, serviceManager ) )
						{
							mock.Value = "test 1";
							vm.AddWorkspaceCommand.Execute( null );
							mock.Value = "test1.txt";
							vm.AddItemCommand.Execute( null );

							mock.Value = "test 2";
							vm.AddWorkspaceCommand.Execute( null );
							mock.Value = "test2.txt";
							vm.AddItemCommand.Execute( null );

							mock.Value = "test 2";
							vm.AddWorkspaceCommand.Execute( null );
							mock.Value = "test2.txt";
							vm.AddItemCommand.Execute( null );

							vm.Save();
						}
					}
				}
			}
		}
	}
}
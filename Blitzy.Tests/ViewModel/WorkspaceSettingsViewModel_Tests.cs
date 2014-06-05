// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();
			WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( baseVM );

			Assert.IsFalse( vm.AddItemCommand.CanExecute( null ) );

			vm.SelectedWorkspace = new Blitzy.Model.Workspace();
			vm.Workspaces.Add( vm.SelectedWorkspace );

			Assert.IsTrue( vm.AddItemCommand.CanExecute( null ) );

			TextInputServiceMock mock = new TextInputServiceMock();
			DialogServiceManager.RegisterService( typeof( TextInputService ), mock );
			mock.Value = null;

			vm.AddItemCommand.Execute( null );
			Assert.AreEqual( 0, vm.SelectedWorkspace.Items.Count );

			mock.Value = "test";
			vm.AddItemCommand.Execute( null );
			Assert.AreEqual( 1, vm.SelectedWorkspace.Items.Count );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddWorkspaceTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();
			WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( baseVM );

			Assert.IsTrue( vm.AddWorkspaceCommand.CanExecute( null ) );

			TextInputServiceMock mock = new TextInputServiceMock();
			DialogServiceManager.RegisterService( typeof( TextInputService ), mock );

			mock.Value = null;
			vm.AddWorkspaceCommand.Execute( null );
			Assert.AreEqual( 0, vm.Workspaces.Count );

			mock.Value = "test";
			vm.AddWorkspaceCommand.Execute( null );
			Assert.AreEqual( 1, vm.Workspaces.Count );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeleteItemTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();
			WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( baseVM );

			Assert.IsFalse( vm.RemoveItemCommand.CanExecute( null ) );

			vm.SelectedWorkspace = new Blitzy.Model.Workspace();
			vm.Workspaces.Add( vm.SelectedWorkspace );

			Assert.IsFalse( vm.RemoveItemCommand.CanExecute( null ) );

			vm.SelectedItem = new Blitzy.Model.WorkspaceItem();
			vm.SelectedWorkspace.Items.Add( vm.SelectedItem );

			Assert.IsTrue( vm.RemoveItemCommand.CanExecute( null ) );

			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			mock.Result = System.Windows.MessageBoxResult.No;

			vm.RemoveItemCommand.Execute( null );
			Assert.AreEqual( 1, vm.SelectedWorkspace.Items.Count );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			vm.RemoveItemCommand.Execute( null );
			Assert.AreEqual( 0, vm.SelectedWorkspace.Items.Count );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DeleteWorkspaceTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel();
			baseVM.Settings = new Blitzy.Model.Settings( Connection );
			baseVM.Reset();
			WorkspaceSettingsViewModel vm = new WorkspaceSettingsViewModel( baseVM );

			Assert.IsFalse( vm.DeleteWorkspaceCommand.CanExecute( null ) );

			vm.SelectedWorkspace = new Blitzy.Model.Workspace();
			vm.Workspaces.Add( vm.SelectedWorkspace );
			Assert.IsTrue( vm.DeleteWorkspaceCommand.CanExecute( null ) );

			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			mock.Result = System.Windows.MessageBoxResult.No;

			vm.DeleteWorkspaceCommand.Execute( null );
			Assert.IsNotNull( vm.SelectedWorkspace );
			Assert.AreEqual( 1, vm.Workspaces.Count );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			vm.DeleteWorkspaceCommand.Execute( null );
			Assert.IsNull( vm.SelectedWorkspace );
			Assert.AreEqual( 0, vm.Workspaces.Count );
		}
	}
}
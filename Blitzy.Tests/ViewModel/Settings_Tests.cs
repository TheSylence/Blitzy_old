// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class Settings_Tests : TestBase
	{
		private SettingsViewModel VM;

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddExcludeTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			DialogServiceManager.RegisterService( typeof( TextInputService ), mock );

			Assert.IsFalse( VM.AddExcludeCommand.CanExecute( null ) );
			VM.SelectedFolder = new Folder();
			Assert.IsTrue( VM.AddExcludeCommand.CanExecute( null ) );

			mock.Text = null;
			VM.AddExcludeCommand.Execute( null );
			Assert.AreEqual( 0, VM.SelectedFolder.Excludes.Count );

			mock.Text = "test";
			VM.AddExcludeCommand.Execute( null );
			CollectionAssert.Contains( VM.SelectedFolder.Excludes, "test" );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddFolderTest()
		{
			StringServiceMock mock = new StringServiceMock();
			mock.Value = "C:\\temp";
			DialogServiceManager.RegisterService( typeof( SelectFolderService ), mock );

			Assert.AreEqual( 0, VM.Settings.Folders.Count() );
			VM.AddFolderCommand.Execute( null );
			Assert.AreEqual( 1, VM.Settings.Folders.Where( f => f.Path == "C:\\temp" ).Count() );

			Assert.AreEqual( 1, VM.Settings.Folders.Count() );
			mock.Value = null;
			VM.AddFolderCommand.Execute( null );
			Assert.AreEqual( 1, VM.Settings.Folders.Count() );

			mock.Value = "C:\\temp2";
			VM.AddFolderCommand.Execute( null );
			Assert.AreEqual( 1, VM.Settings.Folders.Where( f => f.Path == "C:\\temp2" ).Count() );

			Assert.AreEqual( 1, VM.Settings.Folders.Where( f => f.ID == 1 ).Count() );
			Assert.AreEqual( 1, VM.Settings.Folders.Where( f => f.ID == 2 ).Count() );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddRuleTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			DialogServiceManager.RegisterService( typeof( TextInputService ), mock );

			Assert.IsFalse( VM.AddRuleCommand.CanExecute( null ) );
			VM.SelectedFolder = new Folder();
			Assert.IsTrue( VM.AddRuleCommand.CanExecute( null ) );

			mock.Text = null;
			VM.AddRuleCommand.Execute( null );
			Assert.AreEqual( 0, VM.SelectedFolder.Rules.Count );

			mock.Text = "test";
			VM.AddRuleCommand.Execute( null );
			CollectionAssert.Contains( VM.SelectedFolder.Rules, "test" );
		}

		[TestInitialize]
		public override void BeforeTestRun()
		{
			base.BeforeTestRun();

			VM = new SettingsViewModel();
			VM.Settings = new Blitzy.Model.Settings( Connection );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DefaultsTest()
		{
			VM.Settings.SetValue( SystemSetting.MaxMatchingItems, 123 );

			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			VM.DefaultsCommand.Execute( null );
			Assert.AreEqual( 123, VM.Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			VM.DefaultsCommand.Execute( null );
			Assert.AreEqual( 20, VM.Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void LatestVersionTest()
		{
			AutoResetEvent evt = new AutoResetEvent( false );
			Messenger.Default.Register<VersionCheckMessage>( this, ( msg ) =>
				{
					evt.Set();
				} );

			Assert.IsNull( VM.LatestVersion );
			VM.UpdateCheckCommand.Execute( null );

			evt.WaitOne();

			Assert.IsNotNull( VM.LatestVersion );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveExcludeTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			Assert.IsFalse( VM.RemoveExcludeCommand.CanExecute( null ) );
			VM.SelectedFolder = new Folder();
			Assert.IsFalse( VM.RemoveExcludeCommand.CanExecute( null ) );
			VM.SelectedFolder.Excludes.Add( "test" );
			VM.SelectedExclude = "test";
			Assert.IsTrue( VM.RemoveExcludeCommand.CanExecute( null ) );

			VM.RemoveExcludeCommand.Execute( null );
			CollectionAssert.Contains( VM.SelectedFolder.Excludes, "test" );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			VM.RemoveExcludeCommand.Execute( null );
			CollectionAssert.DoesNotContain( VM.SelectedFolder.Excludes, "test" );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveFolderTest()
		{
			Folder folder = new Folder();
			folder.ID = 123;
			folder.Path = "C:\\temp";
			VM.Settings.Folders.Add( folder );

			Assert.IsFalse( VM.RemoveFolderCommand.CanExecute( null ) );
			VM.SelectedFolder = folder;
			Assert.IsTrue( VM.RemoveFolderCommand.CanExecute( null ) );

			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			VM.RemoveFolderCommand.Execute( null );

			Assert.AreSame( folder, VM.SelectedFolder );
			CollectionAssert.Contains( VM.Settings.Folders, folder );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			VM.RemoveFolderCommand.Execute( null );
			Assert.IsNull( VM.SelectedFolder );
			CollectionAssert.DoesNotContain( VM.Settings.Folders, folder );

			VM.Reset();
			VM.SaveCommand.Execute( null );

			VM = new SettingsViewModel();
			VM.Settings = new Settings( Connection );

			CollectionAssert.DoesNotContain( VM.Settings.Folders, folder );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveRuleTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

			Assert.IsFalse( VM.RemoveRuleCommand.CanExecute( null ) );
			VM.SelectedFolder = new Folder();
			Assert.IsFalse( VM.RemoveRuleCommand.CanExecute( null ) );
			VM.SelectedFolder.Rules.Add( "test" );
			VM.SelectedRule = "test";
			Assert.IsTrue( VM.RemoveRuleCommand.CanExecute( null ) );

			VM.RemoveRuleCommand.Execute( null );
			CollectionAssert.Contains( VM.SelectedFolder.Rules, "test" );

			mock.Result = System.Windows.MessageBoxResult.Yes;
			VM.RemoveRuleCommand.Execute( null );
			CollectionAssert.DoesNotContain( VM.SelectedFolder.Rules, "test" );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void StandardCommandsTest()
		{
			Assert.IsTrue( VM.CancelCommand.CanExecute( null ) );
			Assert.IsTrue( VM.DefaultsCommand.CanExecute( null ) );
			Assert.IsTrue( VM.UpdateCheckCommand.CanExecute( null ) );

			Assert.IsFalse( VM.UpdateCatalogCommand.CanExecute( null ) );
			VM.Settings.Folders.Add( new Folder() );
			Assert.IsTrue( VM.UpdateCatalogCommand.CanExecute( null ) );
		}
	}
}
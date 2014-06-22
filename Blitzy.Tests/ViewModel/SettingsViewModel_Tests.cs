// $Id$

using System;
using System.Linq;
using System.Windows.Documents;
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
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class SettingsViewModel_Tests : TestBase
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

			mock.Value = null;
			VM.AddExcludeCommand.Execute( null );
			Assert.AreEqual( 0, VM.SelectedFolder.Excludes.Count );

			mock.Value = "test";
			VM.AddExcludeCommand.Execute( null );
			CollectionAssert.Contains( VM.SelectedFolder.Excludes, "test" );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddFolderTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
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

			mock.Value = null;
			VM.AddRuleCommand.Execute( null );
			Assert.AreEqual( 0, VM.SelectedFolder.Rules.Count );

			mock.Value = "test";
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
		public void CancelTest()
		{
			bool closed = false;
			VM.RequestClose += ( s, e ) => closed = true;

			VM.CancelCommand.Execute( null );

			Assert.IsTrue( closed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CatalogBuildTest()
		{
			VM.Settings.Folders.Add( new Folder() );
			VM.CatalogBuilder = new CatalogBuilder( new Settings( Connection ) );

			bool received = false;
			Messenger.Default.Register<InternalCommandMessage>( this, ( msg ) => received = true );

			VM.UpdateCatalogCommand.Execute( null );
			Assert.IsTrue( received );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CatalogTest()
		{
			VM.Reset();
			VM.CatalogBuilder = new CatalogBuilder( VM.Settings );
			VM.CatalogBuilder.ItemsProcessed = 123;
			VM.CatalogBuilder.ItemsSaved = 456;
			VM.CatalogBuilder.ProgressStep = CatalogProgressStep.Parsing;
			DateTime oldDate = VM.LastCatalogBuild;
			Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.BuildStarted ) );
			Assert.IsTrue( VM.IsCatalogBuilding );

			Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) );
			Assert.AreEqual( VM.CatalogBuilder.ItemsProcessed, VM.CatalogItemsProcessed );

			VM.CatalogBuilder.ProgressStep = CatalogProgressStep.Saving;
			Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) );
			Assert.AreEqual( VM.CatalogBuilder.ItemsSaved, VM.CatalogItemsProcessed );

			Messenger.Default.Send( new CatalogStatusMessage( CatalogStatus.BuildFinished ) );
			Assert.IsFalse( VM.IsCatalogBuilding );
			Assert.AreNotEqual( oldDate, VM.LastCatalogBuild );
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
		public void DownloadUpdateTest()
		{
			Assert.IsFalse( VM.DownloadUpdateCommand.CanExecute( null ) );

			Uri downloadLink = null;
			VM.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), downloadLink, "123", 123, new System.Collections.Generic.Dictionary<Version, string>() );
			Assert.IsFalse( VM.DownloadUpdateCommand.CanExecute( null ) );

			downloadLink = new Uri( "http://localhost/file.exe" );
			VM.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), downloadLink, "123", 123, new System.Collections.Generic.Dictionary<Version, string>() );
			Assert.IsTrue( VM.DownloadUpdateCommand.CanExecute( null ) );

			CallCheckServiceMock mock = new CallCheckServiceMock();
			DialogServiceManager.RegisterService( typeof( DownloadService ), mock );
			VM.DownloadUpdateCommand.Execute( null );

			Assert.IsTrue( mock.WasCalled );
			Assert.IsInstanceOfType( mock.Parameter, typeof( DownloadServiceParameters ) );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			PropertyChangedListener listener = new PropertyChangedListener( VM );
			listener.Exclude<SettingsViewModel>( vm => vm.ApiDatabase );
			listener.Exclude<SettingsViewModel>( vm => vm.BlitzyLicense );
			listener.Exclude<SettingsViewModel>( vm => vm.Changelog );
			listener.Exclude<SettingsViewModel>( vm => vm.CatalogBuilder );
			listener.Exclude<SettingsViewModel>( vm => vm.CurrentVersion );
			listener.Exclude<SettingsViewModel>( vm => vm.Settings );
			Assert.IsTrue( listener.TestProperties() );
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
			Assert.IsTrue( VM.ViewChangelogCommand.CanExecute( null ) );

			Assert.IsFalse( VM.UpdateCatalogCommand.CanExecute( null ) );
			VM.Settings.Folders.Add( new Folder() );
			VM.CatalogBuilder = new CatalogBuilder( new Settings( Connection ) );
			Assert.IsTrue( VM.UpdateCatalogCommand.CanExecute( null ) );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ViewChangelogTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			DialogServiceManager.RegisterService( typeof( ViewChangelogService ), mock );

			VM.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, null, null, null, 0, null );
			VM.ViewChangelogCommand.Execute( null );

			Assert.IsTrue( mock.WasCalled );
			Assert.IsInstanceOfType( mock.Parameter, typeof( btbapi.VersionInfo ) );
		}
	}
}
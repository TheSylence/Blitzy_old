using System;
using System.Linq;
using System.Threading.Tasks;
using Blitzy.btbapi;
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
	public class SettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void AddExcludeTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						Assert.IsFalse( vm.AddExcludeCommand.CanExecute( null ) );
						using( vm.SelectedFolder = new Folder() )
						{
							Assert.IsTrue( vm.AddExcludeCommand.CanExecute( null ) );

							mock.Value = null;
							vm.AddExcludeCommand.Execute( null );
							Assert.AreEqual( 0, vm.SelectedFolder.Excludes.Count );

							mock.Value = "test";
							vm.AddExcludeCommand.Execute( null );
							CollectionAssert.Contains( vm.SelectedFolder.Excludes, "test" );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddFolderTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( SelectFolderService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						mock.Value = "C:\\temp";

						Assert.AreEqual( 0, vm.Settings.Folders.Count() );
						vm.AddFolderCommand.Execute( null );
						Assert.AreEqual( 1, vm.Settings.Folders.Where( f => f.Path == "C:\\temp" ).Count() );

						Assert.AreEqual( 1, vm.Settings.Folders.Count() );
						mock.Value = null;
						vm.AddFolderCommand.Execute( null );
						Assert.AreEqual( 1, vm.Settings.Folders.Count() );

						mock.Value = "C:\\temp2";
						vm.AddFolderCommand.Execute( null );
						Assert.AreEqual( 1, vm.Settings.Folders.Where( f => f.Path == "C:\\temp2" ).Count() );

						Assert.AreEqual( 1, vm.Settings.Folders.Where( f => f.ID == 1 ).Count() );
						Assert.AreEqual( 1, vm.Settings.Folders.Where( f => f.ID == 2 ).Count() );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void AddRuleTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						Assert.IsFalse( vm.AddRuleCommand.CanExecute( null ) );
						using( vm.SelectedFolder = new Folder() )
						{
							Assert.IsTrue( vm.AddRuleCommand.CanExecute( null ) );

							mock.Value = null;
							vm.AddRuleCommand.Execute( null );
							Assert.AreEqual( 0, vm.SelectedFolder.Rules.Count );

							mock.Value = "test";
							vm.AddRuleCommand.Execute( null );
							CollectionAssert.Contains( vm.SelectedFolder.Rules, "test" );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CancelTest()
		{
			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, null ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						bool closed = false;
						vm.RequestClose += ( s, e ) => closed = true;

						vm.CancelCommand.Execute( null );

						Assert.IsTrue( closed );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CatalogBuildTest()
		{
			Messenger messenger = new Messenger();
			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, null, messenger ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						using( Folder folder = new Folder() )
						{
							vm.Settings.Folders.Add( folder );
							using( vm.CatalogBuilder = new CatalogBuilder( ConnectionFactory, vm.Settings ) )
							{
								bool received = false;
								messenger.Register<InternalCommandMessage>( this, ( msg ) => received = true );

								vm.UpdateCatalogCommand.Execute( null );
								Assert.IsTrue( received );
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void CatalogTest()
		{
			Messenger messenger = new Messenger();

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, null, messenger ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						vm.Reset();
						using( vm.CatalogBuilder = new CatalogBuilder( ConnectionFactory, vm.Settings ) )
						{
							vm.CatalogBuilder.ItemsProcessed = 123;
							vm.CatalogBuilder.ItemsSaved = 456;
							vm.CatalogBuilder.ProgressStep = CatalogProgressStep.Parsing;
							DateTime oldDate = vm.LastCatalogBuild;
							messenger.Send( new CatalogStatusMessage( CatalogStatus.BuildStarted ) );
							Assert.IsTrue( vm.IsCatalogBuilding );

							messenger.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) );
							Assert.AreEqual( vm.CatalogBuilder.ItemsProcessed, vm.CatalogItemsProcessed );

							vm.CatalogBuilder.ProgressStep = CatalogProgressStep.Saving;
							messenger.Send( new CatalogStatusMessage( CatalogStatus.ProgressUpdated ) );
							Assert.AreEqual( vm.CatalogBuilder.ItemsSaved, vm.CatalogItemsProcessed );

							messenger.Send( new CatalogStatusMessage( CatalogStatus.BuildFinished ) );
							Assert.IsFalse( vm.IsCatalogBuilding );
							Assert.AreNotEqual( oldDate, vm.LastCatalogBuild );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DefaultsTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						vm.Settings.SetValue( SystemSetting.MaxMatchingItems, 123 );

						vm.DefaultsCommand.Execute( null );
						Assert.AreEqual( 123, vm.Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) );

						mock.Result = System.Windows.MessageBoxResult.Yes;
						vm.DefaultsCommand.Execute( null );
						Assert.AreEqual( 20, vm.Settings.GetValue<int>( SystemSetting.MaxMatchingItems ) );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadUpdateTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( DownloadService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						Assert.IsFalse( vm.DownloadUpdateCommand.CanExecute( null ) );

						Uri downloadLink = null;
						vm.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), downloadLink, "123", 123, new System.Collections.Generic.Dictionary<Version, string>(), null );
						Assert.IsFalse( vm.DownloadUpdateCommand.CanExecute( null ) );

						downloadLink = new Uri( "http://localhost/file.exe" );
						vm.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), downloadLink, "123", 123, new System.Collections.Generic.Dictionary<Version, string>(), null );
						Assert.IsTrue( vm.DownloadUpdateCommand.CanExecute( null ) );

						vm.DownloadUpdateCommand.Execute( null );

						Assert.IsTrue( mock.WasCalled );
						Assert.IsInstanceOfType( mock.Parameter, typeof( DownloadServiceParameters ) );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PluginDialogTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( PluginSettingsService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						vm.PluginsDialogCommand.Execute( null );
						Assert.IsTrue( mock.WasCalled );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PluginMessageTest()
		{
			Messenger messenger = new Messenger();

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, null, messenger ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						vm.Reset();
						MockPlugin plugin = new MockPlugin();

						messenger.Send<PluginMessage>( new PluginMessage( plugin, PluginAction.Enabled ) );
						Assert.IsNotNull( vm.PluginPages.FirstOrDefault( x => x.Plugin == plugin ) );

						messenger.Send<PluginMessage>( new PluginMessage( plugin, PluginAction.Disabled ) );
						Assert.IsNull( vm.PluginPages.FirstOrDefault( x => x.Plugin == plugin ) );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, null ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						PropertyChangedListener listener = new PropertyChangedListener( vm );
						listener.Exclude<SettingsViewModel>( v => v.BlitzyLicense );
						listener.Exclude<SettingsViewModel>( v => v.Changelog );
						listener.Exclude<SettingsViewModel>( v => v.CatalogBuilder );
						listener.Exclude<SettingsViewModel>( v => v.CurrentVersion );
						listener.Exclude<SettingsViewModel>( v => v.Settings );
						listener.Exclude<SettingsViewModel>( v => v.PluginManager );
						Assert.IsTrue( listener.TestProperties() );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveExcludeTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
					{
						MockPluginHost host = new MockPluginHost( vm.Settings );

						using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
						{
							Assert.IsFalse( vm.RemoveExcludeCommand.CanExecute( null ) );
							using( vm.SelectedFolder = new Folder() )
							{
								Assert.IsFalse( vm.RemoveExcludeCommand.CanExecute( null ) );
								vm.SelectedFolder.Excludes.Add( "test" );
								vm.SelectedExclude = "test";
								Assert.IsTrue( vm.RemoveExcludeCommand.CanExecute( null ) );

								vm.RemoveExcludeCommand.Execute( null );
								CollectionAssert.Contains( vm.SelectedFolder.Excludes, "test" );

								mock.Result = System.Windows.MessageBoxResult.Yes;
								vm.RemoveExcludeCommand.Execute( null );
								CollectionAssert.DoesNotContain( vm.SelectedFolder.Excludes, "test" );
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveFolderTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			using( Folder folder = new Folder() )
			{
				folder.ID = 123;
				folder.Path = "C:\\temp";

				using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
				{
					using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
					{
						MockPluginHost host = new MockPluginHost( vm.Settings );

						using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
						{
							vm.Settings.Folders.Add( folder );

							Assert.IsFalse( vm.RemoveFolderCommand.CanExecute( null ) );
							vm.SelectedFolder = folder;
							Assert.IsTrue( vm.RemoveFolderCommand.CanExecute( null ) );

							vm.RemoveFolderCommand.Execute( null );

							Assert.AreSame( folder, vm.SelectedFolder );
							CollectionAssert.Contains( vm.Settings.Folders, folder );

							mock.Result = System.Windows.MessageBoxResult.Yes;
							vm.RemoveFolderCommand.Execute( null );
							Assert.IsNull( vm.SelectedFolder );
							CollectionAssert.DoesNotContain( vm.Settings.Folders, folder );

							vm.Reset();
							vm.SaveCommand.Execute( null );
						}
					}
				}

				using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory ) )
				{
					using( vm.Settings = new Settings( ConnectionFactory ) )
					{
						CollectionAssert.DoesNotContain( vm.Settings.Folders, folder );
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RemoveRuleTest()
		{
			MessageBoxServiceMock mock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), mock );

			using( SettingsViewModel vm = new SettingsViewModel( null, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						Assert.IsFalse( vm.RemoveRuleCommand.CanExecute( null ) );
						using( vm.SelectedFolder = new Folder() )
						{
							Assert.IsFalse( vm.RemoveRuleCommand.CanExecute( null ) );
							vm.SelectedFolder.Rules.Add( "test" );
							vm.SelectedRule = "test";
							Assert.IsTrue( vm.RemoveRuleCommand.CanExecute( null ) );

							vm.RemoveRuleCommand.Execute( null );
							CollectionAssert.Contains( vm.SelectedFolder.Rules, "test" );

							mock.Result = System.Windows.MessageBoxResult.Yes;
							vm.RemoveRuleCommand.Execute( null );
							CollectionAssert.DoesNotContain( vm.SelectedFolder.Rules, "test" );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void StandardCommandsTest()
		{
			using( SettingsViewModel vm = new SettingsViewModel( null ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						Assert.IsTrue( vm.CancelCommand.CanExecute( null ) );
						Assert.IsTrue( vm.DefaultsCommand.CanExecute( null ) );
						Assert.IsTrue( vm.UpdateCheckCommand.CanExecute( null ) );
						Assert.IsTrue( vm.ViewChangelogCommand.CanExecute( null ) );
						Assert.IsTrue( vm.PluginsDialogCommand.CanExecute( null ) );

						Assert.IsFalse( vm.UpdateCatalogCommand.CanExecute( null ) );
						using( Folder folder = new Folder() )
						{
							vm.Settings.Folders.Add( folder );
							using( vm.CatalogBuilder = new CatalogBuilder( ConnectionFactory, vm.Settings ) )
							{
								Assert.IsTrue( vm.UpdateCatalogCommand.CanExecute( null ) );
							}
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void UpdateCheckTest()
		{
			using( SettingsViewModel vm = new SettingsViewModel( null, null ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						using( ShimsContext.Create() )
						{
							Blitzy.Model.Fakes.ShimUpdateChecker.AllInstances.CheckVersionBoolean = ( checker, b ) =>
								{
									return Task.Run<VersionInfo>( () =>
										{
											return new VersionInfo( System.Net.HttpStatusCode.BadRequest, null, null, null, 0, null, null );
										} );
								};
							vm.UpdateCheckAsync().Wait();
							Assert.IsTrue( vm.VersionCheckError );

							Blitzy.Model.Fakes.ShimUpdateChecker.AllInstances.CheckVersionBoolean = ( checker, b ) =>
							{
								return Task.Run<VersionInfo>( () =>
								{
									return new VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), new Uri( "http://localhost/file.name" ), "123", 123, new System.Collections.Generic.Dictionary<Version, string>(), null );
								} );
							};

							vm.UpdateCheckAsync().Wait();
							Assert.IsFalse( vm.VersionCheckError );
						}
					}
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ViewChangelogTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( ViewChangelogService ), mock );
			using( SettingsViewModel vm = new SettingsViewModel( ConnectionFactory, serviceManager ) )
			{
				using( vm.Settings = new Blitzy.Model.Settings( ConnectionFactory ) )
				{
					MockPluginHost host = new MockPluginHost( vm.Settings );

					using( vm.PluginManager = new Plugin.PluginManager( host, ConnectionFactory ) )
					{
						vm.PluginManager.LoadPlugins();
						vm.LatestVersionInfo = new btbapi.VersionInfo( System.Net.HttpStatusCode.OK, null, null, null, 0, null, null );
						vm.ViewChangelogCommand.Execute( null );

						Assert.IsTrue( mock.WasCalled );
						Assert.IsInstanceOfType( mock.Parameter, typeof( btbapi.VersionInfo ) );
					}
				}
			}
		}
	}
}
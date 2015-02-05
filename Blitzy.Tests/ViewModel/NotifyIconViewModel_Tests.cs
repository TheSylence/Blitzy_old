// $Id$

using System;
using Blitzy.btbapi;
using Blitzy.Messages;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class NotifyIconViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void IconTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				using( vm.MainVm = new MainViewModel() )
				{
					vm.Reset();

					Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Finished, null, null ) );
					Assert.IsTrue( vm.IconSource.Contains( "TrayIcon.ico" ) );

					Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Error, null, null ) );
					Assert.IsTrue( vm.IconSource.Contains( "TrayIconFailure.ico" ) );

					Messenger.Default.Send<CommandMessage>( new CommandMessage( CommandStatus.Executing, null, null ) );
					Assert.IsTrue( vm.IconSource.Contains( "CommandExecuting.ico" ) );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );

				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void QuitTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				using( vm.MainVm = new MainViewModel() )
				{
					bool quit = false;
					Messenger.Default.Register<InternalCommandMessage>( this, msg => quit = msg.Command.Equals( "quit" ) );

					Assert.IsTrue( vm.QuitCommand.CanExecute( null ) );
					vm.QuitCommand.Execute( null );
					Assert.IsTrue( quit );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SettingsTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( SettingsService ), mock );

			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				using( vm.MainVm = new MainViewModel( serviceManager ) )
				{
					Assert.IsTrue( vm.SettingsCommand.CanExecute( null ) );
					vm.SettingsCommand.Execute( null );

					Assert.IsTrue( mock.WasCalled );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ShowTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				using( vm.MainVm = new MainViewModel() )
				{
					bool shown = false;
					vm.MainVm.RequestShow += ( s, e ) => shown = true;

					Assert.IsTrue( vm.ShowCommand.CanExecute( null ) );
					vm.ShowCommand.Execute( null );

					Assert.IsTrue( shown );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void VersionCheckTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				vm.Reset();

				Version currentVersion = new Version( 1, 0 );
				Version latestVersion = new Version( 1, 0 );

				VersionInfo versionInfo = new VersionInfo( System.Net.HttpStatusCode.OK, latestVersion, new Uri( "http://localhost" ), "", 0, new System.Collections.Generic.Dictionary<Version, string>(), null );
				VersionCheckMessage msg = new VersionCheckMessage( currentVersion, versionInfo, false );

				BalloonTipMessage msgReceived = null;
				Messenger.Default.Register<BalloonTipMessage>( this, m => msgReceived = m );

				Messenger.Default.Send( msg );
				Assert.IsNull( msgReceived );

				currentVersion = new Version( 0, 9 );
				msg = new VersionCheckMessage( currentVersion, versionInfo, false );
				Messenger.Default.Send( msg );
				Assert.IsNotNull( msgReceived );
				Assert.AreSame( msg, msgReceived.Token );

				msgReceived = null;
				currentVersion = new Version( 1, 0 );
				msg = new VersionCheckMessage( currentVersion, versionInfo, true );
				Messenger.Default.Send( msg );
				Assert.IsNotNull( msgReceived );
				Assert.IsNull( msgReceived.Token );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void VisibleTest()
		{
			using( NotifyIconViewModel vm = new NotifyIconViewModel() )
			{
				using( vm.MainVm = new MainViewModel() )
				{
					vm.MainVm.Settings.SetDefaults();

					Assert.IsTrue( vm.Visible );
					vm.MainVm.Settings.SetValue( Blitzy.Model.SystemSetting.TrayIcon, false );
					Assert.IsFalse( vm.Visible );
				}
			}
		}
	}
}
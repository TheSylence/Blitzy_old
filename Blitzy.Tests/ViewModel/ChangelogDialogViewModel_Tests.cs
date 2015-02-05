// $Id$

using System;
using System.Collections.Generic;
using System.Net;
using Blitzy.btbapi;
using Blitzy.Tests.Mocks;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ChangelogDialogViewModel_Tests
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void ChangelogTest()
		{
			using( ChangelogDialogViewModel vm = new ChangelogDialogViewModel() )
			{
				Version currentVersion = new Version( 1, 2, 3, 4 );
				Uri downloadLink = new Uri( "http://localhost/file/path" );
				string md5 = "md5";
				long size = 12345;
				Dictionary<Version, string> changes = new Dictionary<Version, string>();
				changes.Add( new Version( 1, 2 ), "Changes 1.2" );
				changes.Add( new Version( 1, 3 ), "Changes 1.3" );
				changes.Add( new Version( 2, 1 ), "Changes 2.1" );

				vm.LatestVersionInfo = new VersionInfo( HttpStatusCode.OK, currentVersion, downloadLink, md5, size, changes );

				string expected = "<u>Changes in Version 2.1:</u><br />Changes 2.1" + Environment.NewLine + Environment.NewLine + Environment.NewLine
					+ "<u>Changes in Version 1.3:</u><br />Changes 1.3" + Environment.NewLine + Environment.NewLine + Environment.NewLine
					+ "<u>Changes in Version 1.2:</u><br />Changes 1.2" + Environment.NewLine + Environment.NewLine + Environment.NewLine;

				Assert.AreEqual( expected, vm.Changelog );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadTest()
		{
			using( ChangelogDialogViewModel vm = new ChangelogDialogViewModel() )
			{
				Assert.IsFalse( vm.DownloadCommand.CanExecute( null ) );

				vm.LatestVersionInfo = new VersionInfo( HttpStatusCode.OK, new Version(), new Uri( "http://localhost" ), string.Empty, 123, new Dictionary<Version, string>() );
				Assert.IsTrue( vm.DownloadCommand.CanExecute( null ) );

				CallCheckServiceMock mock = new CallCheckServiceMock();
				DialogServiceManager.RegisterService( typeof( DownloadService ), mock );

				vm.DownloadCommand.Execute( null );
				Assert.IsTrue( mock.WasCalled );
				Assert.IsInstanceOfType( mock.Parameter, typeof( DownloadServiceParameters ) );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( ChangelogDialogViewModel vm = new ChangelogDialogViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );
				Assert.IsTrue( listener.TestProperties() );
			}
		}
	}
}
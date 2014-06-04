// $Id$

using System;
using System.Collections.Generic;
using System.Net;
using Blitzy.Tests.Mocks;
using Blitzy.ViewModel.Dialogs;
using btbapi;
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
			ChangelogDialogViewModel vm = new ChangelogDialogViewModel();

			Version currentVersion = new Version( 1, 2, 3, 4 );
			Uri downloadLink = new Uri( "http://localhost/file/path" );
			string md5 = "md5";
			ulong size = 12345;
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
}
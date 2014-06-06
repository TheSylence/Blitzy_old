// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewServices;
using btbapi;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class UpdateChecker_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void DownloadLatestVersionTest()
		{
			DelegateServiceMock mock = new DelegateServiceMock();
			DialogServiceManager.RegisterService( typeof( DownloadService ), mock );

			bool called = false;
			mock.Action = ( parameter ) =>
			{
				DownloadServiceParameters args = parameter as DownloadServiceParameters;
				Assert.IsNotNull( args );

				Assert.AreEqual( "1234", args.MD5 );
				Assert.AreEqual( 1234, args.FileSize );
				Assert.AreEqual( new Uri( "http://example.com" ).AbsoluteUri, args.DownloadLink.AbsoluteUri );
				called = true;
				return null;
			};

			VersionInfo info = new VersionInfo( System.Net.HttpStatusCode.OK, null, new Uri( "http://example.com" ), "1234", 1234, new Dictionary<Version, string>() );
			UpdateChecker.Instance.DownloadLatestVersion( info );

			Assert.IsTrue( called );
		}
	}
}
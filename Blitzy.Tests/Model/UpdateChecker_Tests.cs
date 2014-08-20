// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewServices;
using btbapi;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.QualityTools.Testing.Fakes;
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

			VersionInfo info = new VersionInfo( System.Net.HttpStatusCode.OK, null, new Uri( "http://example.com" ), "1234", 1234, new Dictionary<Version, string>(), null );
			UpdateChecker.Instance.DownloadLatestVersion( info );

			Assert.IsTrue( called );
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateCheckTest()
		{
			using( ShimsContext.Create() )
			{
				btbapi.Fakes.ShimAPI.AllInstances.CheckVersionStringVersionBoolean = ( api, str, ver, force ) =>
					{
						return Task.Run<VersionInfo>( () => new VersionInfo( System.Net.HttpStatusCode.BadRequest, null, null, null, 0, null, null ) );
					};

				Task<VersionInfo> t = UpdateChecker.Instance.CheckVersion();
				t.Wait();

				Assert.AreNotEqual( System.Net.HttpStatusCode.OK, t.Result.Status );

				btbapi.Fakes.ShimAPI.AllInstances.CheckVersionStringVersionBoolean = ( api, str, ver, force ) =>
				{
					return Task.Run<VersionInfo>( () => new VersionInfo( System.Net.HttpStatusCode.OK, new Version( 1, 2 ), new Uri( "http://localhost/file.name" ), "123", 123, new Dictionary<Version, string>(), null ) );
				};

				bool finished = false;
				Messenger.Default.Register<VersionCheckMessage>( this, msg =>
					{
						finished = true;

						Assert.AreEqual( System.Net.HttpStatusCode.OK, msg.VersionInfo.Status );
						Assert.AreEqual( 123L, msg.VersionInfo.Size );
						Assert.AreEqual( "123", msg.VersionInfo.MD5 );
						Assert.AreEqual( new Version( 1, 2 ), msg.VersionInfo.LatestVersion );
						Assert.AreEqual( new Uri( "http://localhost/file.name" ), msg.VersionInfo.DownloadLink );
					} );

				t = UpdateChecker.Instance.CheckVersion();
				t.Wait();

				Assert.IsTrue( finished );

				Assert.AreEqual( System.Net.HttpStatusCode.OK, t.Result.Status );
				Assert.AreEqual( 123L, t.Result.Size );
				Assert.AreEqual( "123", t.Result.MD5 );
				Assert.AreEqual( new Version( 1, 2 ), t.Result.LatestVersion );
				Assert.AreEqual( new Uri( "http://localhost/file.name" ), t.Result.DownloadLink );
			}
		}
	}
}
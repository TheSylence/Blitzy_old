// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.ViewModel.Dialogs;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class DownloadDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadCorruptedTest()
		{
			DownloadDialogViewModel vm = new DownloadDialogViewModel();

			StringBuilder sb = new StringBuilder();
			sb.Append( "Hello World" );
			for( int i = 0; i < 10000; ++i )
			{
				sb.Append( i.ToString() );
			}

			string content = sb.ToString();

			vm.DownloadSize = content.Length;
			vm.DownloadLink = "file:///" + Directory.GetCurrentDirectory().Replace( '\\', '/' ) + '/' + "test.txt";
			vm.TargetPath = Path.GetTempFileName();

			using( ShimsContext.Create() )
			{
				System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncStringHttpCompletionOption = ( client, link, options ) =>
					{
						return Task.Run<HttpResponseMessage>( () =>
						{
							return new System.Net.Http.Fakes.StubHttpResponseMessage( System.Net.HttpStatusCode.OK )
							{
								Content = new ByteArrayContent( Encoding.ASCII.GetBytes( content ) )
							};
						} );
					};

				//System.Net.Http.Fakes.ShimHttpContent.AllInstances.ReadAsStreamAsync = ( c ) =>
				//	{
				//		return Task.Run<Stream>( () =>
				//		{
				//			return new MemoryStream( Encoding.ASCII.GetBytes( content ) );
				//		} );
				//	};

				bool received = false;
				Messenger.Default.Register<DownloadStatusMessage>( this, MessageTokens.DownloadCorrupted, msg =>
					{
						received = true;
					} );

				vm.StartDownload().Wait();

				Assert.IsTrue( received );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			DownloadDialogViewModel vm = new DownloadDialogViewModel();
			PropertyChangedListener listener = new PropertyChangedListener( vm );
			Assert.IsTrue( listener.TestProperties() );
		}
	}
}
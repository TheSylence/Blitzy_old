// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Fakes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Tests.Mocks.Services;
using Blitzy.Utility;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
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
		public void CancelTest()
		{
			using( DownloadDialogViewModel vm = new DownloadDialogViewModel() )
			{
				Assert.IsFalse( vm.CancelCommand.CanExecute( null ) );

				vm.DownloadSize = 124;
				vm.DownloadLink = "file:///" + Directory.GetCurrentDirectory().Replace( '\\', '/' ) + '/' + "test.txt";
				vm.TargetPath = Path.GetTempFileName();

				using( ShimsContext.Create() )
				{
					System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncStringHttpCompletionOption = ( client, link, options ) =>
						{
							return Task.Run<HttpResponseMessage>( () =>
							{
								Thread.Sleep( 500 );
								return new System.Net.Http.Fakes.StubHttpResponseMessage( System.Net.HttpStatusCode.OK )
								{
									Content = new ByteArrayContent( Encoding.ASCII.GetBytes( "Hello World" ) )
								};
							} );
						};

					bool deleted = false;
					System.IO.Fakes.ShimFile.DeleteString = ( s ) => deleted = true;

					Task t = vm.StartDownload();
					Thread.Sleep( 250 );
					Assert.IsTrue( vm.CancelCommand.CanExecute( null ) );
					vm.CancelCommand.Execute( null );
					t.Wait();

					Assert.IsTrue( deleted );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadCorruptedTest()
		{
			using( DownloadDialogViewModel vm = new DownloadDialogViewModel() )
			{
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

					bool received = false;
					Messenger.Default.Register<DownloadStatusMessage>( this, MessageTokens.DownloadCorrupted, msg =>
						{
							received = true;
						} );

					vm.StartDownload().Wait();

					Assert.IsTrue( received );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadFailedTest()
		{
			using( DownloadDialogViewModel vm = new DownloadDialogViewModel() )
			{
				vm.DownloadSize = 123;
				vm.DownloadLink = "file:///" + Directory.GetCurrentDirectory().Replace( '\\', '/' ) + '/' + "test.txt";

				using( ShimsContext.Create() )
				{
					System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncStringHttpCompletionOption = ( client, link, options ) =>
					{
						return Task.Run<HttpResponseMessage>( () =>
						{
							return new System.Net.Http.Fakes.StubHttpResponseMessage( System.Net.HttpStatusCode.BadRequest );
						} );
					};

					bool received = false;
					Messenger.Default.Register<DownloadStatusMessage>( this, MessageTokens.DownloadFailed, msg =>
					{
						received = true;
					} );

					vm.StartDownload().Wait();

					Assert.IsTrue( received );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void DownloadTest()
		{
			using( DownloadDialogViewModel vm = new DownloadDialogViewModel() )
			{
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
				using( MD5 md5 = System.Security.Cryptography.MD5.Create() )
				{
					vm.MD5 = BitConverter.ToString( md5.ComputeHash( Encoding.ASCII.GetBytes( content ) ) ).Replace( "-", "" ).ToLower();
				}

				using( ShimsContext.Create() )
				{
					System.Net.Http.Fakes.ShimHttpClient.AllInstances.GetAsyncStringHttpCompletionOption = ( client, link, options ) =>
					{
						return Task.Run<HttpResponseMessage>( () =>
						{
							return new System.Net.Http.Fakes.StubHttpResponseMessage( System.Net.HttpStatusCode.BadRequest );
						} );
					};

					vm.StartDownload().Wait();

					Assert.IsFalse( vm.DownloadSuccessfull );
				}
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( DownloadDialogViewModel vm = new DownloadDialogViewModel() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );
				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RetryCorruptedTest()
		{
			MessageBoxServiceMock msgMock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			CallCheckServiceMock callMock = new CallCheckServiceMock();

			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), msgMock );
			serviceManager.RegisterService( typeof( DownloadService ), callMock );

			using( DownloadDialogViewModel vm = new DownloadDialogViewModel( serviceManager ) )
			{
				vm.Reset();

				bool closed = false;
				vm.RequestClose += ( s, e ) => closed = true;
				DownloadStatusMessage msg = new DownloadStatusMessage( "path", "http://localhost/link", 123, "md5" );

				Messenger.Default.Send<DownloadStatusMessage>( msg, MessageTokens.DownloadCorrupted );
				Assert.IsTrue( closed );
				Assert.IsFalse( callMock.WasCalled );

				closed = false;
				msgMock.Result = System.Windows.MessageBoxResult.Yes;

				Messenger.Default.Send<DownloadStatusMessage>( msg, MessageTokens.DownloadCorrupted );
				Assert.IsTrue( closed );
				Assert.IsTrue( callMock.WasCalled );
				Assert.IsInstanceOfType( callMock.Parameter, typeof( DownloadServiceParameters ) );

				DownloadServiceParameters args = callMock.Parameter as DownloadServiceParameters;
				Assert.AreEqual( msg.TargetPath, args.TargetPath );
				Assert.AreEqual( msg.DownloadLink, args.DownloadLink.AbsoluteUri );
				Assert.AreEqual( msg.MD5, args.MD5 );
				Assert.AreEqual( msg.DownloadSize, args.FileSize );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void RetryFailedTest()
		{
			MessageBoxServiceMock msgMock = new MessageBoxServiceMock( System.Windows.MessageBoxResult.No );
			CallCheckServiceMock callMock = new CallCheckServiceMock();

			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( MessageBoxService ), msgMock );
			serviceManager.RegisterService( typeof( DownloadService ), callMock );

			using( DownloadDialogViewModel vm = new DownloadDialogViewModel( serviceManager ) )
			{
				vm.Reset();

				bool closed = false;
				vm.RequestClose += ( s, e ) => closed = true;
				DownloadStatusMessage msg = new DownloadStatusMessage( "path", "http://localhost/link", 123, "md5" );

				Messenger.Default.Send<DownloadStatusMessage>( msg, MessageTokens.DownloadFailed );
				Assert.IsTrue( closed );
				Assert.IsFalse( callMock.WasCalled );

				closed = false;
				msgMock.Result = System.Windows.MessageBoxResult.Yes;

				Messenger.Default.Send<DownloadStatusMessage>( msg, MessageTokens.DownloadFailed );
				Assert.IsTrue( closed );
				Assert.IsTrue( callMock.WasCalled );
				Assert.IsInstanceOfType( callMock.Parameter, typeof( DownloadServiceParameters ) );

				DownloadServiceParameters args = callMock.Parameter as DownloadServiceParameters;
				Assert.AreEqual( msg.TargetPath, args.TargetPath );
				Assert.AreEqual( msg.DownloadLink, args.DownloadLink.AbsoluteUri );
				Assert.AreEqual( msg.MD5, args.MD5 );
				Assert.AreEqual( msg.DownloadSize, args.FileSize );
			}
		}
	}
}
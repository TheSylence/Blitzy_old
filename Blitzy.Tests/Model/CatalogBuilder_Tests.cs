using System.IO;
using Blitzy.Messages;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class CatalogBuilder_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void BuildTest()
		{
			Settings settings = new Settings( Connection );

			using( Folder folder = new Folder() )
			{
				folder.Path = "builder_test";
				folder.Rules.Add( "*.*" );

				TestHelper.CreateTestFolder( "builder_test" );
				File.AppendAllText( "builder_test/test.txt", "" );
				File.AppendAllText( "builder_test/test1.txt", "" );
				File.AppendAllText( "builder_test/test2.txt", "" );
				File.Copy( "TestData/Blitzy.exe.lnk", "builder_test/Blitzy.exe.lnk" );

				settings.Folders.Add( folder );
			}

			Messenger messenger = new Messenger();
			using( CatalogBuilder builder = new CatalogBuilder( settings, messenger ) )
			{
				bool started = false;
				bool done = false;

				messenger.Register<CatalogStatusMessage>( this, msg =>
					{
						switch( msg.Status )
						{
							case CatalogStatus.BuildStarted:
								started = true;
								break;

							case CatalogStatus.BuildFinished:
								done = true;
								break;
						}
					} );

				builder.Build();

				Assert.IsTrue( started );
				Assert.IsTrue( done );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			Settings settings = new Settings( Connection );
			using( CatalogBuilder builder = new CatalogBuilder( settings ) )
			{
				PropertyChangedListener listener = new PropertyChangedListener( builder );
				Assert.IsTrue( listener.TestProperties() );
			}
		}
	}
}
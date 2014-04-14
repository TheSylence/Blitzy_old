// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	public class CatalogBuilder_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void BuildTest()
		{
			Settings settings = new Settings( Connection );

			Folder folder = new Folder();
			folder.Path = "builder_test";
			folder.Rules.Add( "*.*" );

			TestHelper.CreateTestFolder( "builder_test" );
			File.AppendAllText( "builder_test/test.txt", "" );
			File.AppendAllText( "builder_test/test1.txt", "" );
			File.AppendAllText( "builder_test/test2.txt", "" );

			settings.Folders.Add( folder );
			using( CatalogBuilder builder = new CatalogBuilder( settings ) )
			{
				bool started = false;
				bool done = false;

				Messenger.Default.Register<CatalogStatusMessage>( this, msg =>
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
	}
}
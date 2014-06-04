// $Id$

using System;
using System.Diagnostics;
using System.IO;
using Blitzy.Model.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	// FIXME: This does not work...
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class ShellShortcut_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void LoadTest()
		{
			const string fileName = "TestData/Blitzy.exe.lnk";
			Assert.IsTrue( File.Exists( fileName ) );

			using( ShellShortcut sh = new ShellShortcut( fileName ) )
			{
				Assert.IsNotNull( sh.Path );
				//Assert.AreEqual( "args", sh.Arguments );
				Assert.AreEqual( "Test", sh.Description );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			string fileName = Path.GetFullPath( "test.lnk" );
			string path = Path.GetFullPath( "Blitzy.exe" );
			Assert.IsTrue( File.Exists( path ) );

			using( ShellShortcut sh = new ShellShortcut( fileName ) )
			{
				sh.Path = path;
				sh.WorkingDirectory = Environment.CurrentDirectory;
				sh.Arguments = "args";
				sh.Description = "desc";
				sh.WindowStyle = ProcessWindowStyle.Normal;
				sh.IconPath = path;
				sh.IconIndex = 0;

				sh.Save();
				Assert.IsTrue( File.Exists( fileName ), "File does not exist on disk" );
			}

			using( ShellShortcut sh = new ShellShortcut( fileName ) )
			{
				//Assert.AreEqual( "args", sh.Arguments );
				Assert.AreEqual( "desc", sh.Description );
				Assert.AreEqual( ProcessWindowStyle.Normal, sh.WindowStyle );
				Assert.AreEqual( path, sh.IconPath );
				Assert.AreEqual( 0, sh.IconIndex );
				Assert.AreEqual( Environment.CurrentDirectory, sh.WorkingDirectory );
				Assert.AreEqual( path, sh.Path );
			}
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Blitzy.Model.Shell;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	// FIXME: This does not work...
	[TestClass]
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
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			const string fileName = "test.lnk";
			string path = Path.GetFullPath( "Blitzy.exe" );

			using( ShellShortcut sh = new ShellShortcut( fileName ) )
			{
				sh.Path = path;

				Assert.IsTrue( sh.Save() );
				Assert.IsTrue( File.Exists( fileName ), "File does not exist on disk" );
			}

			using( ShellShortcut sh = new ShellShortcut( fileName ) )
			{
				Assert.AreEqual( path, sh.Path );
			}
		}
	}
}
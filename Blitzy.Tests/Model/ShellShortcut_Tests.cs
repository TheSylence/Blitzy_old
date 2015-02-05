

using System;
using System.Diagnostics;
using System.IO;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ShellShortcut_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void InvalidTest()
		{
			ShellShortcut sh = new ShellShortcut( "non.existing" );
			Assert.IsFalse( sh.Valid );
			Assert.IsNull( sh.Path );
			Assert.IsNull( sh.Arguments );
			Assert.IsNull( sh.Description );
			Assert.IsNull( sh.IconPath );
		}

		[TestMethod, TestCategory( "Model" )]
		public void LoadTest()
		{
			const string fileName = "TestData/Blitzy.exe.lnk";
			Assert.IsTrue( File.Exists( fileName ), Path.GetFullPath( fileName ) );

			ShellShortcut sh = new ShellShortcut( fileName );
			Assert.IsTrue( sh.Valid );
			Assert.IsNotNull( sh.Path );
			Assert.IsTrue( System.IO.Path.IsPathRooted( sh.Path ) );
			Assert.AreEqual( "args", sh.Arguments );
			Assert.AreEqual( "Test", sh.Description );
			Assert.IsNotNull( sh.IconPath );
			Assert.IsTrue( System.IO.Path.IsPathRooted( sh.IconPath ) );
		}
	}
}
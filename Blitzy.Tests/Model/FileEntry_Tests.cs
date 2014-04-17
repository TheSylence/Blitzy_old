// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	public class FileEntry_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void EqualsTest()
		{
			FileEntry entry = new FileEntry( "cmd", "name", "icon", "type", "args" );
			FileEntry other = new FileEntry( "cmd", "other name", "other icon", "other type", "args" );

			Assert.IsTrue( entry.Equals( other ) );
			other = new FileEntry( "cmd", "other name", "other icon", "other type", "args2" );
			Assert.IsFalse( entry.Equals( other ) );

			other = new FileEntry( "cmd2", "other name", "other icon", "other type", "args" );
			Assert.IsFalse( entry.Equals( other ) );

			other = new FileEntry( "cmd2", "other name", "other icon", "other type", "args2" );
			Assert.IsFalse( entry.Equals( other ) );

			Assert.IsFalse( entry.Equals( string.Empty ) );
			Assert.IsFalse( entry.Equals( null ) );
		}
	}
}
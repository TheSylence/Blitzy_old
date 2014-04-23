// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public sealed class TestHelper
	{
		private static Stack<string> TestFolders;

		[AssemblyCleanup()]
		public static void AssemblyCleanup()
		{
			while( TestFolders.Count > 0 )
			{
				string folder = TestFolders.Pop();
				Directory.Delete( folder, true );
			}
		}

		[AssemblyInitialize()]
		public static void AssemblyInit( TestContext context )
		{
			TestFolders = new Stack<string>();
		}

		public static void CreateTestFolder( string folder )
		{
			TestFolders.Push( folder );
			Directory.CreateDirectory( folder );
		}
	}
}
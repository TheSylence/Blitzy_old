// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Folder_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void ExcludeTest()
		{
			Folder f = new Folder();
			f.ID = 1;
			f.Path = "C:\\temp";
			f.Excludes.Add( "ex1" );
			f.Excludes.Add( "ex2" );

			f.Save( Connection );

			f = new Folder();
			f.ID = 1;
			f.Load( Connection );

			CollectionAssert.Contains( f.Excludes, "ex1" );
			CollectionAssert.Contains( f.Excludes, "ex2" );

			f.Excludes.Add( "ex3" );
			f.Excludes.Remove( "ex2" );

			f.Save( Connection );

			f = new Folder();
			f.ID = 1;
			f.Load( Connection );

			CollectionAssert.Contains( f.Excludes, "ex1" );
			CollectionAssert.DoesNotContain( f.Excludes, "ex2" );
			CollectionAssert.Contains( f.Excludes, "ex3" );
		}

		[TestMethod, TestCategory( "Model" )]
		public void GetFilesTest()
		{
			Folder f = new Folder();
			f.Rules.Add( "*.txt" );
			f.Excludes.Add( "exclude*.txt" );
			f.Path = "folder_test";

			TestHelper.CreateTestFolder( "folder_test" );
			File.AppendAllText( "folder_test/test.txt", "" );
			File.AppendAllText( "folder_test/exclude.txt", "" );
			File.AppendAllText( "folder_test/exclude1.txt", "" );
			File.AppendAllText( "folder_test/include.txt", "" );

			TestHelper.CreateTestFolder( "folder_test/rec" );
			File.AppendAllText( "folder_test/rec/rec_test.txt", "" );
			File.AppendAllText( "folder_test/rec/rec_exclude.txt", "" );

			f.IsRecursive = false;

			string[] files = f.GetFiles().Select( file => Path.GetFileName( file ) ).ToArray();
			CollectionAssert.Contains( files, "test.txt", "test.txt" );
			CollectionAssert.Contains( files, "include.txt", "include.txt" );
			CollectionAssert.DoesNotContain( files, "exclude.txt", "exclude.txt" );
			CollectionAssert.DoesNotContain( files, "exclude1.txt", "exclude1.txt" );

			f.IsRecursive = true;
			files = f.GetFiles().Select( file => Path.GetFileName( file ) ).ToArray();

			CollectionAssert.Contains( files, "test.txt", "test.txt" );
			CollectionAssert.Contains( files, "include.txt", "include.txt" );
			CollectionAssert.DoesNotContain( files, "exclude.txt", "exclude.txt" );
			CollectionAssert.DoesNotContain( files, "exclude1.txt", "exclude1.txt" );

			CollectionAssert.DoesNotContain( files, "rec_exclude.txt", "rec_exclude.txt" );
			CollectionAssert.Contains( files, "rec_test.txt", "rec_test.txt" );
		}

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void LoadNonExistingTest()
		{
			Folder f = new Folder();
			f.ID = int.MaxValue;

			f.Load( Connection );
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			Folder obj = new Folder();
			PropertyChangedListener listener = new PropertyChangedListener( obj );
			listener.Exclude<Folder>( o => o.ID );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "Model" )]
		public void RulesTest()
		{
			Folder f = new Folder();
			f.ID = 1;
			f.Path = "C:\\temp";
			f.Rules.Add( "rule1" );
			f.Rules.Add( "rule2" );

			f.Save( Connection );

			f = new Folder();
			f.ID = 1;
			f.Load( Connection );

			CollectionAssert.Contains( f.Rules, "rule1" );
			CollectionAssert.Contains( f.Rules, "rule2" );

			f.Rules.Add( "rule3" );
			f.Rules.Remove( "rule2" );

			f.Save( Connection );

			f = new Folder();
			f.ID = 1;
			f.Load( Connection );

			CollectionAssert.Contains( f.Rules, "rule1" );
			CollectionAssert.DoesNotContain( f.Rules, "rule2" );
			CollectionAssert.Contains( f.Rules, "rule3" );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			Folder f = new Folder();
			Assert.IsFalse( f.ExistsInDatabase );

			f.Path = "C:\\temp";
			f.ID = 1;
			f.IsRecursive = true;

			f.Save( Connection );

			Assert.IsTrue( f.ExistsInDatabase );

			Folder f2 = new Folder();
			f2.ID = 1;
			f2.Load( Connection );

			Assert.IsTrue( f2.ExistsInDatabase );
			Assert.AreEqual( f.Path, f2.Path );
			Assert.AreEqual( f.IsRecursive, f2.IsRecursive );
		}
	}
}
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

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void LoadNonExistingTest()
		{
			Folder f = new Folder();
			f.ID = int.MaxValue;

			f.Load( Connection );
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
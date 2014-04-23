// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebyWebsite_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( TypeLoadException ) )]
		public void LoadNonExistingTest()
		{
			WebyWebsite w = new WebyWebsite();
			w.ID = int.MaxValue;

			w.Load( Connection );
		}

		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			WebyWebsite obj = new WebyWebsite();
			PropertyChangedListener listener = new PropertyChangedListener( obj );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			WebyWebsite w = new WebyWebsite();
			Assert.IsFalse( w.ExistsInDatabase );

			w.Name = "google";
			w.ID = 1;
			w.Description = "This is a test";
			w.URL = "http://google.com/q={0}";

			w.Save( Connection );

			Assert.IsTrue( w.ExistsInDatabase );

			WebyWebsite w2 = new WebyWebsite();
			w2.ID = 1;
			w2.Load( Connection );

			Assert.IsTrue( w2.ExistsInDatabase );
			Assert.AreEqual( w.Name, w2.Name );
			Assert.AreEqual( w.Description, w2.Description );
			Assert.AreEqual( w.URL, w2.URL );
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			WebyWebsite w = new WebyWebsite();
			Assert.IsFalse( w.ExistsInDatabase );

			w.Name = "google";
			w.ID = 1;
			w.Description = "This is a test";
			w.URL = "http://google.com/q={0}";

			w.Save( Connection );
			Assert.IsTrue( w.ExistsInDatabase );

			w.Name = "helloworld";
			w.Save( Connection );

			w = new WebyWebsite();
			w.ID = 1;
			w.Load( Connection );

			Assert.AreEqual( "helloworld", w.Name );
		}
	}
}
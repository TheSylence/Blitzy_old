using System;
using Blitzy.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WebyWebsite_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void PropertyChangedTest()
		{
			using( WebyWebsite obj = new WebyWebsite() )
			{
				PropertyChangedListener listener = new PropertyChangedListener( obj );

				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			using( WebyWebsite w = new WebyWebsite() )
			{
				Assert.IsFalse( w.ExistsInDatabase );
				int id = TestHelper.NextID();

				w.Name = "google";
				w.ID = id;
				w.Description = "This is a test";
				w.URL = "http://google.com/q={0}";

				w.Save( Connection );

				Assert.IsTrue( w.ExistsInDatabase );

				using( WebyWebsite w2 = new WebyWebsite() )
				{
					w2.ID = id;
					w2.Load( Connection );

					Assert.IsTrue( w2.ExistsInDatabase );
					Assert.AreEqual( w.Name, w2.Name );
					Assert.AreEqual( w.Description, w2.Description );
					Assert.AreEqual( w.URL, w2.URL );
				}
			}

			using( WebyWebsite w = new WebyWebsite() )
			{
				w.ID = int.MaxValue;

				ExceptionAssert.Throws<TypeLoadException>( () => w.Load( Connection ) );
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void UpdateTest()
		{
			int id = TestHelper.NextID();

			using( WebyWebsite w = new WebyWebsite() )
			{
				Assert.IsFalse( w.ExistsInDatabase );

				w.Name = "google";
				w.ID = id;
				w.Description = "This is a test";
				w.URL = "http://google.com/q={0}";

				w.Save( Connection );
				Assert.IsTrue( w.ExistsInDatabase );

				w.Name = "helloworld";
				w.Save( Connection );
			}

			using( WebyWebsite w = new WebyWebsite() )
			{
				w.ID = id;
				w.Load( Connection );

				Assert.AreEqual( "helloworld", w.Name );
			}
		}
	}
}
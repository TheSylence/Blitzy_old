

using System.Linq;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class BaseObject_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void DisposeObjectTest()
		{
			using( MockModel obj = new MockModel() )
			{
				MockModel[] objects = Enumerable.Range( 0, 10 ).Select( i => new MockModel() ).ToArray();

				foreach( MockModel o in objects )
				{
					obj.ToDisposeWrapper( o );
				}

				Assert.AreEqual( 10, obj.ObjectsToDispose.Count );

				obj.DisposeObjectWrapper( objects[3] );

				Assert.IsTrue( objects[3].IsDisposed );

				Assert.AreEqual( 9, obj.ObjectsToDispose.Count );
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is what is being tested" ), TestMethod, TestCategory( "Global" )]
		public void DoubleDispose()
		{
			MockModel obj = new MockModel();
			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );

			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );
		}

		[TestMethod, TestCategory( "Global" )]
		public void ToDisposeTest()
		{
			using( MockModel obj = new MockModel() )
			{
				using( MockModel obj2 = new MockModel() )
				{
					MockModel obj3 = obj2.ToDisposeWrapper( obj );

					Assert.AreSame( obj, obj3 );

					obj2.Dispose();

					Assert.IsTrue( obj.IsDisposed );
				}
			}
		}
	}
}
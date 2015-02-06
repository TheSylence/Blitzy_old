

using System.Linq;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ViewModelBaseEx_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void DisposeObjectTest()
		{
			using( MockViewModel obj = new MockViewModel( "container" ) )
			{
				MockViewModel[] objects = Enumerable.Range( 0, 10 ).Select( i => new MockViewModel( i.ToString() ) ).ToArray();

				foreach( MockViewModel o in objects )
				{
					obj.ToDisposeWrapper( o );
				}

				Assert.AreEqual( 10, obj.ObjectsToDispose.Count );

				obj.DisposeObjectWrapper( objects[3] );

				Assert.IsTrue( objects[3].IsDisposed );

				Assert.AreEqual( 9, obj.ObjectsToDispose.Count );
			}
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is exactly what is being tested" ), TestMethod, TestCategory( "ViewModel" )]
		public void DoubleDispose()
		{
			MockViewModel obj = new MockViewModel( "test" );
			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );

			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );
		}

		[System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Usage", "CA2202:Do not dispose objects multiple times" ), TestMethod, TestCategory( "ViewModel" )]
		public void ToDisposeTest()
		{
			using( MockViewModel obj = new MockViewModel( "test" ) )
			{
				using( MockViewModel obj2 = new MockViewModel( "test2" ) )
				{
					MockViewModel obj3 = obj2.ToDisposeWrapper( obj );

					Assert.AreSame( obj, obj3 );

					obj2.Dispose();

					Assert.IsTrue( obj.IsDisposed );
				}
			}
		}
	}
}
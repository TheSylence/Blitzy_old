// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class ViewModelBaseEx_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void DisposeObjectTest()
		{
			MockViewModel obj = new MockViewModel( "container" );

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

		[TestMethod, TestCategory( "ViewModel" )]
		public void DoubleDispose()
		{
			MockViewModel obj = new MockViewModel( "test" );
			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );

			obj.Dispose();
			Assert.IsTrue( obj.IsDisposed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ToDisposeTest()
		{
			MockViewModel obj = new MockViewModel( "test" );

			MockViewModel obj2 = new MockViewModel( "test2" );
			MockViewModel obj3 = obj2.ToDisposeWrapper( obj );

			Assert.AreSame( obj, obj3 );

			obj2.Dispose();

			Assert.IsTrue( obj.IsDisposed );
		}
	}
}
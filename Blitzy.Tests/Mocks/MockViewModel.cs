// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewModel;

namespace Blitzy.Tests.Mocks
{
	internal class MockViewModel : ViewModelBaseEx
	{
		#region Constructor

		public MockViewModel( string name )
		{
			Name = name;
		}

		#endregion Constructor

		#region Methods

		internal void DisposeObjectWrapper( IDisposable obj )
		{
			DisposeObject( obj );
		}

		internal T ToDisposeWrapper<T>( T obj ) where T : IDisposable
		{
			return ToDispose( obj );
		}

		#endregion Methods

		#region Properties

		public string Name { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
using System;
using System.Data.Common;
using Blitzy.Model;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockModel : ModelBase
	{
		public override void Delete( DbConnection connection )
		{
			throw new NotImplementedException();
		}

		public override void Load( DbConnection connection )
		{
			throw new NotImplementedException();
		}

		public override void Save( DbConnection connection )
		{
			throw new NotImplementedException();
		}

		internal void DisposeObjectWrapper( IDisposable obj )
		{
			DisposeObject( obj );
		}

		internal T ToDisposeWrapper<T>( T obj ) where T : IDisposable
		{
			return ToDispose( obj );
		}
	}
}
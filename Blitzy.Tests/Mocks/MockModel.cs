// $Id$

using System;
using Blitzy.Model;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockModel : ModelBase
	{
		public override void Delete( System.Data.SQLite.SQLiteConnection connection )
		{
			throw new NotImplementedException();
		}

		public override void Load( System.Data.SQLite.SQLiteConnection connection )
		{
			throw new NotImplementedException();
		}

		public override void Save( System.Data.SQLite.SQLiteConnection connection )
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
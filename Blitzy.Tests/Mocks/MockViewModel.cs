using System;
using Blitzy.ViewModel;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockViewModel : ViewModelBaseEx
	{
		public MockViewModel( string name )
			: base( null )
		{
			Name = name;
		}

		internal void Debug( string p )
		{
			LogDebug( p );
		}

		internal void DisposeObjectWrapper( IDisposable obj )
		{
			DisposeObject( obj );
		}

		internal void Error( string p )
		{
			LogError( p );
		}

		internal void Fatal( string p )
		{
			LogFatal( p );
		}

		internal void Info( string p )
		{
			LogInfo( p );
		}

		internal T ToDisposeWrapper<T>( T obj ) where T : IDisposable
		{
			return ToDispose( obj );
		}

		internal void Warning( string p )
		{
			LogWarning( p );
		}

		public string Name { get; private set; }
	}
}


using System;
using Blitzy.ViewModel;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockViewModel : ViewModelBaseEx
	{
		#region Constructor

		public MockViewModel( string name )
		{
			Name = name;
		}

		#endregion Constructor

		#region Methods

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

		#endregion Methods

		#region Properties

		public string Name { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
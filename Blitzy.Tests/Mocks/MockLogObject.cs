﻿namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockLogObject : BaseObject
	{
		internal void Debug( string p )
		{
			LogDebug( p );
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

		internal void Warning( string p )
		{
			LogWarning( p );
		}
	}
}
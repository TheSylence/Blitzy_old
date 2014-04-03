// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Tests.Mocks
{
	internal class MockLogObject : LogObject
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
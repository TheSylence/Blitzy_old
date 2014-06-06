// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Blitzy.Utility
{
	[Serializable]
	[ExcludeFromCodeCoverage]
	public class OperationAlreadyStartedException : Exception
	{
		public OperationAlreadyStartedException()
		{
		}

		public OperationAlreadyStartedException( string message )
			: base( message )
		{
		}

		public OperationAlreadyStartedException( string message, Exception inner )
			: base( message, inner )
		{
		}

		protected OperationAlreadyStartedException(
		  SerializationInfo info,
		  StreamingContext context )
			: base( info, context ) { }
	}
}
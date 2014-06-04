// $Id$

using System;

namespace Blitzy.Utility
{
	[Serializable]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
		  System.Runtime.Serialization.SerializationInfo info,
		  System.Runtime.Serialization.StreamingContext context )
			: base( info, context ) { }
	}
}
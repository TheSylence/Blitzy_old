// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Utility
{
	[Serializable]
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
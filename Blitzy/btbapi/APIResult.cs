using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Blitzy.btbapi
{
	/// <summary>
	/// Base class for a result that comes from an API call
	/// </summary>
	public abstract class APIResult
	{
		protected APIResult( HttpStatusCode status, string rawResponse )
		{
			RawResponse = rawResponse;
			Status = status;
		}

		/// <summary>
		/// The Raw and unparsed response of the API
		/// </summary>
		public readonly string RawResponse;

		/// <summary>
		/// HTTP Status of the result.
		/// Always check this before accessing anything from the result.
		/// Only a HTTP 200 means this result is valid.
		/// </summary>
		public readonly HttpStatusCode Status;
	}
}
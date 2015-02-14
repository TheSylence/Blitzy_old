using System;

namespace Blitzy.btbapi
{
	/// <summary>
	/// Information about an Endpoint to the API
	/// </summary>
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public sealed class APIEndPoint
	{
		/// <summary>
		/// Constructs a new APIEndPoint
		/// </summary>
		/// <param name="baseUri">Base URL for this EndPoint</param>
		internal APIEndPoint( Uri baseUri )
		{
			BaseUri = baseUri;
		}

		/// <summary>
		/// The default EndPoint that points to the live system
		/// </summary>
		public static APIEndPoint Default
		{
			get
			{
				return new APIEndPoint( new Uri( "http://btbsoft.org/api/1.0/" ) );
			}
		}

		/// <summary>
		/// An EndPoint that uses the local instance. Use this for testing
		/// </summary>
		public static APIEndPoint Localhost
		{
			get
			{
				return new APIEndPoint( new Uri( "http://localhost/btb/api/1.0/" ) );
			}
		}

		public Uri BaseUri { get; private set; }

		// <summary>
		/// This Endpoint is INVALID and should be only used for testing for INVALID requests
		/// </summary>
		internal static APIEndPoint Test
		{
			get
			{
				return new APIEndPoint( new Uri( "http://localhost/invalid/api.end.point" ) );
			}
		}
	}
}
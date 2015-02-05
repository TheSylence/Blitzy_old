using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.btbapi
{
	/// <summary>
	/// Collection of helper methods
	/// </summary>
	internal static class APIHelper
	{
		#region Methods

		/// <summary>
		/// Builds an url that can be used for making API calls
		/// </summary>
		/// <param name="endpoint">The EndPoint to use</param>
		/// <param name="action">The API action</param>
		/// <param name="args">Arguments to be passed to the URL</param>
		/// <returns>The full URL that can be used to make a call</returns>
		internal static string BuildUrl( APIEndPoint endpoint, string action, Dictionary<string, string> args )
		{
			string url = endpoint.BaseUri + action;

			string q = string.Join( "&", args.Select( kvp => string.Format( "{0}={1}", WebUtility.UrlEncode( kvp.Key ), WebUtility.UrlEncode( kvp.Value ) ) ) );
			if( !string.IsNullOrWhiteSpace( q ) )
			{
				url += "?" + q;
			}

			return url;
		}

		/// <summary>
		/// Genereates the MD5-Hash of a string
		/// </summary>
		/// <param name="str">The string</param>
		/// <returns>The MD5 hash of <paramref name="str"/></returns>
		internal static string MD5( string str )
		{
			if( string.IsNullOrEmpty( str ) )
			{
				return string.Empty;
			}

			MD5 md5 = new MD5CryptoServiceProvider();
			byte[] textToHash = Encoding.Default.GetBytes( str );
			byte[] result = md5.ComputeHash( textToHash );

			return System.BitConverter.ToString( result ).Replace( "-", "" ).ToLower();
		}

		#endregion Methods

		#region Constants

		/// <summary>
		/// Action that is used for reporting an error
		/// </summary>
		internal const string ErrorReportAction = "error_report";

		/// <summary>
		/// Action that is used for checking for updates
		/// </summary>
		internal const string UpdateCheckAction = "version_check";

		#endregion Constants
	}
}
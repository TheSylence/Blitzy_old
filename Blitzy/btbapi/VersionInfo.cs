using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;

namespace Blitzy.btbapi
{
	/// <summary>
	/// Information about the current version of a software
	/// </summary>
	public class VersionInfo : APIResult
	{
		public VersionInfo( HttpStatusCode status, Version currentVersion, Uri downloadLink, string md5, long size, Dictionary<Version, string> changes, string rawResponse )
			: base( status, rawResponse )
		{
			LatestVersion = currentVersion;
			DownloadLink = downloadLink;
			MD5 = md5;
			Size = size;
			if( changes != null )
			{
				ChangeLogs = new ReadOnlyDictionary<Version, string>( changes );
			}
			else
			{
				ChangeLogs = null;
			}
		}

		/// <summary>
		/// A list of changes for all versions from your current version to the latest
		/// (If you did not provide your current version you will only get the changes of the latest version)
		/// </summary>
		public IReadOnlyDictionary<Version, string> ChangeLogs { get; private set; }

		/// <summary>
		/// The location where to download the latest version
		/// </summary>
		public Uri DownloadLink { get; private set; }

		/// <summary>
		/// The latest version available
		/// </summary>
		public Version LatestVersion { get; private set; }

		/// <summary>
		/// MD5 Hash of the download file
		/// </summary>
		public string MD5 { get; private set; }

		/// <summary>
		/// Size in bytes of the download file
		/// </summary>
		public long Size { get; private set; }
	}
}
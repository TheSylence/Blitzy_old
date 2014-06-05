// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class VersionCheckMessage : MessageBase
	{
		public readonly Version CurrentVersion;
		public readonly string DownloadURL;
		public readonly Version LatestVersion;
		public readonly bool ShowIfNewest;

		public VersionCheckMessage( Version currentVersion, Version latestVersion, string url, bool showIfNewest )
		{
			LatestVersion = latestVersion;
			CurrentVersion = currentVersion;
			DownloadURL = url;
			ShowIfNewest = showIfNewest;
		}
	}
}
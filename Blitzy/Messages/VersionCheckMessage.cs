using System;
using System.Diagnostics.CodeAnalysis;
using Blitzy.btbapi;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class VersionCheckMessage : MessageBase
	{
		public VersionCheckMessage( Version currentVersion, VersionInfo versionInfo, bool showIfNewest )
		{
			CurrentVersion = currentVersion;
			ShowIfNewest = showIfNewest;
			VersionInfo = versionInfo;
		}

		public readonly Version CurrentVersion;
		public readonly bool ShowIfNewest;
		public readonly VersionInfo VersionInfo;
	}
}
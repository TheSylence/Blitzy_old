// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using btbapi;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class VersionCheckMessage : MessageBase
	{
		public readonly Version CurrentVersion;
		public readonly bool ShowIfNewest;
		public readonly VersionInfo VersionInfo;

		public VersionCheckMessage( Version currentVersion, VersionInfo versionInfo, bool showIfNewest )
		{
			CurrentVersion = currentVersion;
			ShowIfNewest = showIfNewest;
			VersionInfo = versionInfo;
		}
	}
}
﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Messages
{
	internal class VersionCheckMessage
	{
		public readonly string DownloadURL;

		public readonly bool ShowIfNewest;

		public readonly Version Version;

		public VersionCheckMessage( Version version, string url, bool showIfNewest )
		{
			Version = version;
			DownloadURL = url;
			ShowIfNewest = showIfNewest;
		}
	}
}
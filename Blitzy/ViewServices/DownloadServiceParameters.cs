// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.ViewServices
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class DownloadServiceParameters
	{
		public readonly Uri DownloadLink;

		public readonly long FileSize;

		public readonly string MD5;

		public readonly string TargetPath;

		public DownloadServiceParameters( Uri downloadLink, string targetPath, long fileSize, string md5 )
		{
			DownloadLink = downloadLink;
			TargetPath = targetPath;
			FileSize = fileSize;
			MD5 = md5;
		}
	}
}
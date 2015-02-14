using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	/// <summary>
	/// Message indicating the status of a downloa.
	/// Success or error will be send using a token
	/// </summary>
	internal class DownloadStatusMessage : MessageBase
	{
		[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
		public DownloadStatusMessage( string targetPath, string downloadLink, long downloadSize, string md5 )
		{
			TargetPath = targetPath;
			DownloadLink = downloadLink;
			MD5 = md5;
			DownloadSize = downloadSize;
		}

		public readonly string DownloadLink;
		public readonly long DownloadSize;
		public readonly string MD5;
		public readonly string TargetPath;
	}
}
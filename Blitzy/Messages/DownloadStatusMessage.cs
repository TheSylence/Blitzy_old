// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Messages
{
	/// <summary>
	/// Message indicating the status of a downloa.
	/// Success or error will be send using a token
	/// </summary>
	internal class DownloadStatusMessage
	{
		public readonly string TargetPath;

		public DownloadStatusMessage( string targetPath )
		{
			TargetPath = targetPath;
		}
	}
}
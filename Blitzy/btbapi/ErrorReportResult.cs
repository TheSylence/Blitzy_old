using System;
using System.Net;

namespace Blitzy.btbapi
{
	/// <summary>
	/// Result of an error report
	/// </summary>
	public class ErrorReportResult : APIResult
	{
		public ErrorReportResult( HttpStatusCode status, Uri issueLink, bool alreadyKnown, string rawResponse )
			: base( status, rawResponse )
		{
			IssueLink = issueLink;
			AlreadyKnown = alreadyKnown;
		}

		/// <summary>
		/// Indicating whether this issue has been reported previously
		/// </summary>
		public bool AlreadyKnown { get; private set; }

		/// <summary>
		/// Link to the Issue. If the issue hasn't been
		/// </summary>
		public Uri IssueLink { get; private set; }
	}
}
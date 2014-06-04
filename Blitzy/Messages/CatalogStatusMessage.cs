// $Id$

using System.Diagnostics.CodeAnalysis;

namespace Blitzy.Messages
{
	internal enum CatalogStatus
	{
		BuildStarted,
		ProgressUpdated,
		BuildFinished
	}

	[ExcludeFromCodeCoverage]
	internal class CatalogStatusMessage
	{
		public readonly CatalogStatus Status;

		public CatalogStatusMessage( CatalogStatus status )
		{
			Status = status;
		}
	}
}
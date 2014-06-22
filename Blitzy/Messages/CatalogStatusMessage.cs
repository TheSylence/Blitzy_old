// $Id$

using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	internal enum CatalogStatus
	{
		BuildStarted,
		ProgressUpdated,
		BuildFinished
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class CatalogStatusMessage : MessageBase
	{
		public readonly CatalogStatus Status;

		public CatalogStatusMessage( CatalogStatus status )
		{
			Status = status;
		}
	}
}
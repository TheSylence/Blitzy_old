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
		public CatalogStatusMessage( CatalogStatus status )
		{
			Status = status;
		}

		public readonly CatalogStatus Status;
	}
}
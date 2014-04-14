// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
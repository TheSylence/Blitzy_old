// $Id$

using System.Diagnostics.CodeAnalysis;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class FileDialogParameters
	{
		public readonly string Filter;

		public FileDialogParameters( string filter = null )
		{
			Filter = filter;
		}
	}
}
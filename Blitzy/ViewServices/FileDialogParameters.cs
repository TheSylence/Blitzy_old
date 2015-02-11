using System.Diagnostics.CodeAnalysis;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class FileDialogParameters
	{
		public FileDialogParameters( string filter = null )
		{
			Filter = filter;
		}

		public readonly string Filter;
	}
}
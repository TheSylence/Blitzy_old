// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
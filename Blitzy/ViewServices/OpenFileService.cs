// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class OpenFileService : IDialogService
	{
		#region Methods

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			OpenFileDialog dlg = new OpenFileDialog();

			FileDialogParameters args = parameter as FileDialogParameters;
			if( args != null )
			{
				dlg.Filter = args.Filter;
			}

			if( dlg.ShowDialog( parent ) == true )
			{
				return dlg.FileName;
			}

			return null;
		}

		#endregion Methods
	}
}
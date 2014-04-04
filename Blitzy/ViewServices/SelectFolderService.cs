// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Ookii.Dialogs.Wpf;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SelectFolderService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog();
			dlg.SelectedPath = parameter as string;
			if( dlg.ShowDialog( parent ) == true )
			{
				return dlg.SelectedPath;
			}

			return null;
		}
	}
}
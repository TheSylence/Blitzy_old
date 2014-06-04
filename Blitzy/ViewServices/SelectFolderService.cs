// $Id$

using System.Diagnostics.CodeAnalysis;
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
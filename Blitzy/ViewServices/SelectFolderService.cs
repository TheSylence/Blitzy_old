using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Ookii.Dialogs.Wpf;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SelectFolderService : IViewService
	{
		public object Show( Window parent, object parameter = null )
		{
			VistaFolderBrowserDialog dlg = new VistaFolderBrowserDialog { SelectedPath = parameter as string };
			if( dlg.ShowDialog( parent ) == true )
			{
				return dlg.SelectedPath;
			}

			return null;
		}
	}
}
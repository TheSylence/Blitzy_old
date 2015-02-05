// $Id$

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.btbapi;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class ViewChangelogService : IDialogService
	{
		public object Show( Window parent, object parameter = null )
		{
			VersionInfo args = parameter as VersionInfo;
			if( args == null )
			{
				throw new ArgumentException( "ViewChangelogService needs VersionInfo" );
			}

			ChangelogDialog dlg = new ChangelogDialog { Owner = parent };

			ChangelogDialogViewModel vm = dlg.DataContext as ChangelogDialogViewModel;
			Debug.Assert( vm != null );
			vm.Reset();
			vm.LatestVersionInfo = args;

			return dlg.ShowDialog();
		}
	}
}
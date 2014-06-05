﻿// $Id$

using System;
using System.Diagnostics;
using System.Windows;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel.Dialogs;
using btbapi;

namespace Blitzy.ViewServices
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class ViewChangelogService : IDialogService
	{
		public object Show( Window parent, object parameter = null )
		{
			VersionInfo args = parameter as VersionInfo;
			if( args == null )
			{
				throw new ArgumentException( "ViewChangelogService needs VersionInfo" );
			}

			ChangelogDialog dlg = new ChangelogDialog();
			dlg.Owner = parent;

			ChangelogDialogViewModel vm = dlg.DataContext as ChangelogDialogViewModel;
			Debug.Assert( vm != null );
			vm.LatestVersionInfo = args;

			return dlg.ShowDialog();
		}
	}
}
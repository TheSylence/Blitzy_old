// $Id$

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SettingsService : IDialogService
	{
		public object Show( Window parent, object parameter = null )
		{
			SettingsServiceParameters args = parameter as SettingsServiceParameters;
			if( args == null )
			{
				throw new ArgumentException( "SettingsService needs SettingsServiceParameters" );
			}

			SettingsDialog dlg = new SettingsDialog { Owner = parent };

			SettingsViewModel vm = dlg.DataContext as SettingsViewModel;
			Debug.Assert( vm != null );
			vm.Settings = args.Settings;
			vm.CatalogBuilder = args.Builder;
			vm.Reset();

			return dlg.ShowDialog();
		}
	}
}
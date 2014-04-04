// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SettingsService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			SettingsDialog dlg = new SettingsDialog();
			dlg.Owner = parent;

			SettingsViewModel vm = dlg.DataContext as SettingsViewModel;
			Debug.Assert( vm != null );
			vm.Settings = parameter as Settings;
			vm.Reset();

			return dlg.ShowDialog();
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel;

namespace Blitzy.ViewServices
{
	internal class SettingsService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			SettingsDialog dlg = new SettingsDialog();
			dlg.Owner = parent;

			SettingsViewModel vm = dlg.DataContext as SettingsViewModel;
			vm.Settings = parameter as Settings;

			return dlg.ShowDialog();
		}
	}
}
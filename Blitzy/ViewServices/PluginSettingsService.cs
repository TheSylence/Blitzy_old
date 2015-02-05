

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.ViewServices
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class PluginSettingsService : IViewService
	{
		public object Show( Window parent, object parameter = null )
		{
			Plugin.PluginManager pmgr = parameter as Plugin.PluginManager;
			if( pmgr == null )
			{
				throw new ArgumentException( "PluginSettingService needs PluginManager as parameter" );
			}

			PluginsDialog dlg = new PluginsDialog();
			PluginsDialogViewModel vm = dlg.DataContext as PluginsDialogViewModel;

			Debug.Assert( vm != null );
			vm.PluginManager = pmgr;
			vm.Reset();

			dlg.Owner = parent;

			return dlg.ShowDialog();
		}
	}
}
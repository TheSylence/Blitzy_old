﻿// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel;

namespace Blitzy.ViewServices
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class DownloadService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			DownloadServiceParameters args = parameter as DownloadServiceParameters;
			if( args == null )
			{
				throw new ArgumentException( "DownloadService needs DownloadServiceParameters" );
			}

			DownloadDialog dlg = new DownloadDialog();
			dlg.Owner = parent;

			DownloadDialogViewModel vm = dlg.DataContext as DownloadDialogViewModel;
			Debug.Assert( vm != null );
			vm.DownloadLink = args.DownloadLink.AbsoluteUri;
			vm.MD5 = args.MD5;
			vm.TargetPath = args.TargetPath;
			vm.FileSize = args.FileSize;

			return dlg.ShowDialog();
		}
	}
}
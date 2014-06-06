// $Id$

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class DownloadService : IDialogService
	{
		public object Show( Window parent, object parameter = null )
		{
			DownloadServiceParameters args = parameter as DownloadServiceParameters;
			if( args == null )
			{
				throw new ArgumentException( "DownloadService needs DownloadServiceParameters" );
			}

			DownloadDialog dlg = new DownloadDialog { Owner = parent };

			DownloadDialogViewModel vm = dlg.DataContext as DownloadDialogViewModel;
			Debug.Assert( vm != null );
			vm.Reset();
			vm.DownloadLink = args.DownloadLink.AbsoluteUri;
			vm.MD5 = args.MD5;
			vm.TargetPath = args.TargetPath;
			vm.DownloadSize = args.FileSize;

			vm.StartDownload();
			return dlg.ShowDialog();
		}
	}
}
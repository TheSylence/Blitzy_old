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
	internal class TextInputService : IViewService
	{
		public object Show( Window parent, object parameter = null )
		{
			TextInputParameter args = parameter as TextInputParameter;
			if( args == null )
			{
				throw new ArgumentException( "TextInputService erwartet ein TextInputParameter objekt als Parameter" );
			}

			TextInputDialog dlg = new TextInputDialog { Owner = parent };

			TextInputDialogViewModel vm = dlg.DataContext as TextInputDialogViewModel;
			Debug.Assert( vm != null );
			vm.Caption = args.Caption;
			vm.Input = args.Input;
			vm.LabelText = args.LabelText;

			if( dlg.ShowDialog() == true )
			{
				return vm.Input;
			}

			return null;
		}
	}
}
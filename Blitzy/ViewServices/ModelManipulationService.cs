// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.Model;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal abstract class ModelManipulationService<TModel, TDialog> : IDataManipulationService
		where TModel : ModelBase
		where TDialog : Window
	{
		public Type ModelType
		{
			get { return typeof( TModel ); }
		}

		public object Create( System.Windows.Window parent )
		{
			TDialog wnd = (TDialog)Activator.CreateInstance( typeof( TDialog ) );
			wnd.Owner = parent;

			DialogViewModelBase<TModel> vm = wnd.DataContext as DialogViewModelBase<TModel>;
			vm.Reset();
			vm.New = true;

			if( wnd.ShowDialog() == true )
			{
				return vm.Model;
			}

			return null;
		}

		public bool Edit( System.Windows.Window parent, object obj )
		{
			TDialog wnd = (TDialog)Activator.CreateInstance( typeof( TDialog ) );
			wnd.Owner = parent;

			DialogViewModelBase<TModel> vm = wnd.DataContext as DialogViewModelBase<TModel>;
			vm.Reset();
			vm.Model = obj as TModel;
			vm.New = false;

			if( wnd.ShowDialog() == true )
			{
				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
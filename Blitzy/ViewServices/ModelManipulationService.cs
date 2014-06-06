// $Id$

using System;
using System.Diagnostics;
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

		public object Create( Window parent )
		{
			TDialog wnd = (TDialog)Activator.CreateInstance( typeof( TDialog ) );
			wnd.Owner = parent;

			DialogViewModelBase<TModel> vm = wnd.DataContext as DialogViewModelBase<TModel>;
			Debug.Assert( vm != null );
			vm.Reset();
			vm.New = true;

			if( wnd.ShowDialog() == true )
			{
				return vm.Model;
			}

			return null;
		}

		public bool Edit( Window parent, object obj )
		{
			TDialog wnd = (TDialog)Activator.CreateInstance( typeof( TDialog ) );
			wnd.Owner = parent;

			DialogViewModelBase<TModel> vm = wnd.DataContext as DialogViewModelBase<TModel>;
			Debug.Assert( vm != null );
			vm.Reset();
			vm.Model = obj as TModel;
			vm.New = false;

			return wnd.ShowDialog() == true;
		}
	}
}
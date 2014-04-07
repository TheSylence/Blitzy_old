using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using Blitzy.ViewModel;

namespace Blitzy.View
{
	public class CloseableView : Window
	{
		#region Constructor

		private IRequestCloseViewModel Model;

		public CloseableView()
		{
			DataContextChanged += CloseableView_DataContextChanged;
			Closed += CloseableView_Closed;
		}

		private void CloseableView_Closed( object sender, EventArgs e )
		{
			if( Model != null )
			{
				Model.RequestClose -= model_RequestClose;
			}
		}

		private void CloseableView_DataContextChanged( object sender, DependencyPropertyChangedEventArgs e )
		{
			IRequestCloseViewModel model = e.NewValue as IRequestCloseViewModel;

			if( model != null )
			{
				Model = model;
				model.RequestClose += model_RequestClose;
				model.RequestHide += model_RequestHide;
			}
		}

		private void model_RequestClose( object sender, CloseViewEventArgs e )
		{
			try
			{
				DialogResult = e.Result;
			}
			catch( InvalidOperationException )
			{
			}

			Close();
		}

		private void model_RequestHide( object sender, EventArgs e )
		{
			Hide();
		}

		#endregion Constructor
	}
}
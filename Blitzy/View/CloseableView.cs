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
				model.RequestShow += model_RequestShow;
			}
		}

		private void model_RequestClose( object sender, CloseViewEventArgs e )
		{
			if( OwnedWindows.Count > e.MaxWindowCount )
			{
				return;
			}

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
			if( Hidden != null )
			{
				Hidden( this, EventArgs.Empty );
			}
		}

		private void model_RequestShow( object sender, EventArgs e )
		{
			Show();
			if( Shown != null )
			{
				Shown( this, EventArgs.Empty );
			}
		}

		#endregion Constructor

		#region Events

		public event EventHandler Hidden;

		public event EventHandler Shown;

		#endregion Events
	}
}
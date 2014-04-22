using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Blitzy.Messages;
using Blitzy.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Controls
{
	/// <summary>
	/// Interaction logic for HistoryWindow.xaml
	/// </summary>
	public partial class HistoryWindow : UserControl
	{
		#region Constructor

		public HistoryWindow()
		{
			InitializeComponent();

			Messenger.Default.Register<HistoryMessage>( this, msg => OnMessage( msg ) );
		}

		#endregion Constructor

		#region Methods

		private void OnMessage( HistoryMessage msg )
		{
			switch( msg.Type )
			{
				case HistoryMessageType.Show:
					this.Visibility = System.Windows.Visibility.Visible;
					VM.Manager = msg.History;
					break;

				case HistoryMessageType.Hide:
					this.Visibility = System.Windows.Visibility.Collapsed;
					break;
			}
		}

		#endregion Methods

		#region Properties

		private HistoryViewModel VM
		{
			get
			{
				return DataContext as HistoryViewModel;
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
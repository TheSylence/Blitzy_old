using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using Blitzy.Messages;
using Blitzy.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Controls
{
	/// <summary>
	/// Interaction logic for HistoryWindow.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class HistoryWindow
	{
		#region Constructor

		public HistoryWindow()
		{
			InitializeComponent();

			Messenger.Default.Register<HistoryMessage>( this, OnMessage );
		}

		#endregion Constructor

		#region Methods

		private void OnMessage( HistoryMessage msg )
		{
			switch( msg.Type )
			{
				case HistoryMessageType.Show:
					Visibility = Visibility.Visible;
					VM.Manager = msg.History;
					break;

				case HistoryMessageType.Hide:
					Visibility = Visibility.Collapsed;
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
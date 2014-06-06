using System.Diagnostics.CodeAnalysis;
using Blitzy.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class MainWindow : CloseableView
	{
		public MainWindow()
		{
			InitializeComponent();

			Activated += ( s, e ) => txtInput.Focus();

			Messenger.Default.Register<HistoryMessage>( this, msg =>
			{
				if( msg.Type == HistoryMessageType.Hide )
				{
					txtInput.Focus();
				}
			} );
		}
	}
}
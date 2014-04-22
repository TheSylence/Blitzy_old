using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

			Activated += ( s, e ) =>
				{
					txtInput.Focus();
				};

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
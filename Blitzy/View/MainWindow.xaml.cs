using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Interop;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.ViewModel;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class MainWindow
	{
		private HotKeyHost KeyHost;

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

			IntPtr handle = new WindowInteropHelper( this ).EnsureHandle();
			HwndSource source = HwndSource.FromHwnd( handle );
			source.AddHook( WndProc );
			KeyHost = new HotKeyHost( source );

			Messenger.Default.Register<HotKeyMessage>( this, msg =>
			{
				KeyHost.AddHotKey( new HotKey( msg.Key, msg.Modifiers, true ) );
			} );

			KeyHost.HotKeyPressed += ( s, e ) => Show();

			MainViewModel vm = DataContext as MainViewModel;
			vm.RegisterHotKey();
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == SingleInstance.WM_SHOWFIRSTINSTANCE )
			{
				Show();
				handled = true;
			}

			return IntPtr.Zero;
		}
	}
}
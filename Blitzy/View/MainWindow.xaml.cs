using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows.Input;
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
		private HotKey CurrentHotKey;
		private HotKeyHost KeyHost;

		public MainWindow()
		{
			InitializeComponent();

			Activated += ( s, e ) => FocusInput();
			Shown += ( s, e ) => FocusInput();

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
				if( CurrentHotKey != null )
				{
					KeyHost.RemoveHotKey( CurrentHotKey );
				}

				CurrentHotKey = new HotKey( msg.Key, msg.Modifiers, true );
				KeyHost.AddHotKey( CurrentHotKey );
			} );

			KeyHost.HotKeyPressed += ( s, e ) => Show();

			MainViewModel vm = DataContext as MainViewModel;
			vm.RegisterHotKey();
		}

		private void FocusInput()
		{
			txtInput.Focus();
			FocusManager.SetFocusedElement( this, txtInput );
		}

		private IntPtr WndProc( IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
		{
			if( msg == SingleInstance.WM_SHOWFIRSTINSTANCE )
			{
				if( Visibility == System.Windows.Visibility.Visible )
				{
					Hide();
				}
				else
				{
					// TODO: Sometimes the window loses focus when shown?
					Show();
				}
				handled = true;
			}

			return IntPtr.Zero;
		}
	}
}
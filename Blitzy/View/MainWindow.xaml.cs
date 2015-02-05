using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.View
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class MainWindow : IDisposable
	{
		private HotKey CurrentHotKey;
		private HotKeyHost KeyHost;

		public MainWindow()
		{
			InitializeComponent();

			Activated += ( s, e ) => FocusInput();
			Shown += ( s, e ) => FocusInput();
			GotFocus += ( s, e ) => FocusInput();
			GotKeyboardFocus += ( s, e ) => FocusInput();

			Messenger.Default.Register<HistoryMessage>( this, msg =>
			{
				if( msg.Type == HistoryMessageType.Hide )
				{
					FocusInput();
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

				try
				{
					KeyHost.AddHotKey( CurrentHotKey );
				}
				catch( HotKeyAlreadyRegisteredException )
				{
					string text = "HotKeyAlreadyRegistered".Localize();
					string caption = "HotKeyError".Localize();
					MessageBoxParameter args = new MessageBoxParameter( text, caption, MessageBoxButton.OK, MessageBoxImage.Warning );
					DialogServiceManager.Show<MessageBoxService>( args );
				}
			} );

			KeyHost.HotKeyPressed += ( s, e ) =>
			{
				if( Visibility == System.Windows.Visibility.Visible )
				{
					Hide();
				}
				else
				{
					Show();
					Activate();
				}
			};

			MainViewModel vm = DataContext as MainViewModel;
			vm.RegisterHotKey();

			Hide();
		}

		~MainWindow()
		{
			Dispose( false );
		}

		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		private void Dispose( bool disposing )
		{
			if( disposing )
			{
				KeyHost.Dispose();
			}
		}

		private void FocusInput()
		{
			FocusManager.SetFocusedElement( this, txtInput );
			txtInput.Focus();
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
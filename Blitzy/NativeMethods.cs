// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy
{
	internal static class NativeMethods
	{
		public const int HWND_BROADCAST = 0xffff;
		public const int SW_SHOWNORMAL = 1;

		#region user32.dll

		[DllImport( "user32.dll" )]
		public static extern bool DestroyIcon( IntPtr hIcon );

		[return: MarshalAs( UnmanagedType.Bool )]
		[DllImport( "user32.dll", SetLastError = true )]
		public static extern bool PostMessage( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );

		[DllImport( "user32" )]
		public static extern int RegisterWindowMessage( [MarshalAs( UnmanagedType.LPWStr )] string message );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool SetForegroundWindow( IntPtr hWnd );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		public static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

		[DllImport( "user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true )]
		internal static extern int RegisterHotKey( IntPtr hwnd, int id, int modifiers, int key );

		[DllImport( "user32.dll" )]
		internal static extern IntPtr SendMessage( IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam );

		[DllImport( "user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true )]
		internal static extern int UnregisterHotKey( IntPtr hwnd, int id );

		#endregion user32.dll

		public static int RegisterWindowMessage( string format, params object[] args )
		{
			string message = String.Format( CultureInfo.InvariantCulture, format, args );
			return RegisterWindowMessage( message );
		}

		public static void ShowToFront( IntPtr window )
		{
			ShowWindow( window, SW_SHOWNORMAL );
			SetForegroundWindow( window );
		}

		[DllImport( "dwmapi.dll", PreserveSig = false )]
		internal static extern int DwmIsCompositionEnabled();

		[DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
		internal static extern IntPtr ExtractIcon( IntPtr hInst, string lpszExeFileName, int nIconIndex );

		[DllImport( "shell32.dll", CharSet = CharSet.Auto )]
		internal static extern int ExtractIconEx( [MarshalAs( UnmanagedType.LPWStr )]string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons );

		[DllImport( "msi.dll", CharSet = CharSet.Unicode )]
		internal static extern UInt32 MsiGetComponentPath( string szProduct, string szComponent, [Out] StringBuilder lpPathBuf, ref UInt32 pcchBuf );

		[DllImport( "msi.dll", CharSet = CharSet.Unicode )]
		internal static extern UInt32 MsiGetShortcutTargetW( string szShortcutTarget, [Out] StringBuilder szProductCode, [Out] StringBuilder szFeatureId, [Out] StringBuilder szComponentCode );
	}
}
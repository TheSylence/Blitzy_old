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
	internal class NativeMethods : INativeMethods
	{
		#region INativeMethods

		public override bool DestroyIcon_Wrapper( IntPtr hIcon )
		{
			return DestroyIcon( hIcon );
		}

		public override int DwmIsCompositionEnabled_Wrapper()
		{
			return DwmIsCompositionEnabled();
		}

		public override IntPtr ExtractIcon_Wrapper( IntPtr hInst, string lpszExeFileName, int nIconIndex )
		{
			return ExtractIcon( hInst, lpszExeFileName, nIconIndex );
		}

		public override int ExtractIconEx_Wrapper( string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons )
		{
			return ExtractIconEx( stExeFileName, nIconIndex, ref phiconLarge, ref phiconSmall, nIcons );
		}

		public override uint MsiGetComponentPath_Wrapper( string szProduct, string szComponent, StringBuilder lpPathBuf, ref uint pcchBuf )
		{
			return MsiGetComponentPath( szProduct, szComponent, lpPathBuf, ref pcchBuf );
		}

		public override uint MsiGetShortcutTargetW_Wrapper( string szShortcutTarget, StringBuilder szProductCode, StringBuilder szFeatureId, StringBuilder szComponentCode )
		{
			return MsiGetShortcutTargetW( szShortcutTarget, szProductCode, szFeatureId, szComponentCode );
		}

		public override bool PostMessage_Wrapper( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam )
		{
			return PostMessage( hwnd, msg, wparam, lparam );
		}

		public override int RegisterHotKey_Wrapper( IntPtr hwnd, int id, int modifiers, int key )
		{
			return RegisterHotKey( hwnd, id, modifiers, key );
		}

		public override int RegisterWindowMessage_Wrapper( string format, params object[] args )
		{
			string message = String.Format( CultureInfo.InvariantCulture, format, args );
			return RegisterWindowMessage( message );
		}

		public override int RegisterWindowMessage_Wrapper( string message )
		{
			return RegisterWindowMessage( message );
		}

		public override IntPtr SendMessage_Wrapper( IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam )
		{
			return SendMessage( hWnd, Msg, wParam, lParam );
		}

		public override bool SetForegroundWindow_Wrapper( IntPtr hWnd )
		{
			return SetForegroundWindow( hWnd );
		}

		public override void ShowToFront_Wrapper( IntPtr window )
		{
			ShowWindow( window, SW_SHOWNORMAL );
			SetForegroundWindow( window );
		}

		public override bool ShowWindow_Wrapper( IntPtr hWnd, int nCmdShow )
		{
			return ShowWindow( hWnd, nCmdShow );
		}

		public override int UnregisterHotKey_Wrapper( IntPtr hwnd, int id )
		{
			return UnregisterHotKey( hwnd, id );
		}

		#endregion INativeMethods

		#region user32.dll

		[DllImport( "user32.dll" )]
		private static extern bool DestroyIcon( IntPtr hIcon );

		[return: MarshalAs( UnmanagedType.Bool )]
		[DllImport( "user32.dll", SetLastError = true )]
		private static extern bool PostMessage( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );

		[DllImport( "user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true )]
		private static extern int RegisterHotKey( IntPtr hwnd, int id, int modifiers, int key );

		[DllImport( "user32" )]
		private static extern int RegisterWindowMessage( [MarshalAs( UnmanagedType.LPWStr )] string message );

		[DllImport( "user32.dll" )]
		private static extern IntPtr SendMessage( IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool SetForegroundWindow( IntPtr hWnd );

		[DllImport( "user32.dll" )]
		[return: MarshalAs( UnmanagedType.Bool )]
		private static extern bool ShowWindow( IntPtr hWnd, int nCmdShow );

		[DllImport( "user32.dll", CharSet = CharSet.Ansi, SetLastError = true, ExactSpelling = true )]
		private static extern int UnregisterHotKey( IntPtr hwnd, int id );

		#endregion user32.dll

		#region shell32.dll

		[DllImport( "shell32.dll", CharSet = CharSet.Unicode )]
		private static extern IntPtr ExtractIcon( IntPtr hInst, string lpszExeFileName, int nIconIndex );

		[DllImport( "shell32.dll", CharSet = CharSet.Auto )]
		private static extern int ExtractIconEx( [MarshalAs( UnmanagedType.LPWStr )]string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons );

		#endregion shell32.dll

		#region msi.dll

		[DllImport( "msi.dll", CharSet = CharSet.Unicode )]
		private static extern UInt32 MsiGetComponentPath( string szProduct, string szComponent, [Out] StringBuilder lpPathBuf, ref UInt32 pcchBuf );

		[DllImport( "msi.dll", CharSet = CharSet.Unicode )]
		private static extern UInt32 MsiGetShortcutTargetW( string szShortcutTarget, [Out] StringBuilder szProductCode, [Out] StringBuilder szFeatureId, [Out] StringBuilder szComponentCode );

		#endregion msi.dll

		#region dwmapi.dll

		[DllImport( "dwmapi.dll", PreserveSig = false )]
		private static extern int DwmIsCompositionEnabled();

		#endregion dwmapi.dll
	}
}
// $Id$

using System;
using System.Text;

namespace Blitzy
{
	public abstract class INativeMethods
	{
		public const int HWND_BROADCAST = 0xffff;
		public const int SW_SHOWNORMAL = 1;

		public static INativeMethods Instance { get; internal set; }

		#region Methods

		public abstract bool DestroyIcon_Wrapper( IntPtr hIcon );

		public abstract int DwmIsCompositionEnabled_Wrapper();

		public abstract IntPtr ExtractIcon_Wrapper( IntPtr hInst, string lpszExeFileName, int nIconIndex );

		public abstract int ExtractIconEx_Wrapper( string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons );

		public abstract UInt32 MsiGetComponentPath_Wrapper( string szProduct, string szComponent, StringBuilder lpPathBuf, ref UInt32 pcchBuf );

		public abstract UInt32 MsiGetShortcutTargetW_Wrapper( string szShortcutTarget, StringBuilder szProductCode, StringBuilder szFeatureId, StringBuilder szComponentCode );

		public abstract bool PostMessage_Wrapper( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam );

		public abstract int RegisterHotKey_Wrapper( IntPtr hwnd, int id, int modifiers, int key );

		public abstract int RegisterWindowMessage_Wrapper( string format, params object[] args );

		public abstract int RegisterWindowMessage_Wrapper( string message );

		public abstract IntPtr SendMessage_Wrapper( IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam );

		public abstract bool SetForegroundWindow_Wrapper( IntPtr hWnd );

		public abstract void ShowToFront_Wrapper( IntPtr window );

		public abstract bool ShowWindow_Wrapper( IntPtr hWnd, int nCmdShow );

		public abstract int UnregisterHotKey_Wrapper( IntPtr hwnd, int id );

		#endregion Methods
	}
}
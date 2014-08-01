// $Id$

using System;
using System.Text;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class NativeMethodsMock : INativeMethods
	{
		#region Methods

		public override bool DestroyIcon_Wrapper( IntPtr hIcon )
		{
			throw new NotImplementedException();
		}

		public override int DwmIsCompositionEnabled_Wrapper()
		{
			throw new NotImplementedException();
		}

		public override IntPtr ExtractIcon_Wrapper( IntPtr hInst, string lpszExeFileName, int nIconIndex )
		{
			throw new NotImplementedException();
		}

		public override int ExtractIconEx_Wrapper( string stExeFileName, int nIconIndex, ref IntPtr phiconLarge, ref IntPtr phiconSmall, int nIcons )
		{
			throw new NotImplementedException();
		}

		public override uint MsiGetComponentPath_Wrapper( string szProduct, string szComponent, StringBuilder lpPathBuf, ref uint pcchBuf )
		{
			throw new NotImplementedException();
		}

		public override uint MsiGetShortcutTargetW_Wrapper( string szShortcutTarget, StringBuilder szProductCode, StringBuilder szFeatureId, StringBuilder szComponentCode )
		{
			throw new NotImplementedException();
		}

		public override bool PostMessage_Wrapper( IntPtr hwnd, int msg, IntPtr wparam, IntPtr lparam )
		{
			throw new NotImplementedException();
		}

		public override int RegisterHotKey_Wrapper( IntPtr hwnd, int id, int modifiers, int key )
		{
			throw new NotImplementedException();
		}

		public override int RegisterWindowMessage_Wrapper( string format, params object[] args )
		{
			throw new NotImplementedException();
		}

		public override int RegisterWindowMessage_Wrapper( string message )
		{
			throw new NotImplementedException();
		}

		public override IntPtr SendMessage_Wrapper( IntPtr hWnd, int Msg, IntPtr wParam, IntPtr lParam )
		{
			return OnSendMessage( hWnd, Msg, wParam, lParam );
		}

		public override bool SetForegroundWindow_Wrapper( IntPtr hWnd )
		{
			throw new NotImplementedException();
		}

		public override int SHGetStockIconInfo_Wrapper( View.StockIconIdentifier identifier, View.StockIconOptions flags, ref View.StockIconInfo info )
		{
			throw new NotImplementedException();
		}

		public override void ShowToFront_Wrapper( IntPtr window )
		{
			throw new NotImplementedException();
		}

		public override bool ShowWindow_Wrapper( IntPtr hWnd, int nCmdShow )
		{
			throw new NotImplementedException();
		}

		public override int UnregisterHotKey_Wrapper( IntPtr hwnd, int id )
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Delegates

		internal Func<IntPtr, int, IntPtr, IntPtr, IntPtr> OnSendMessage;

		#endregion Delegates
	}
}
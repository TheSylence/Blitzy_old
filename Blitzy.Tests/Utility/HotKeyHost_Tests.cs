// $Id$

using System;
using System.Linq;
using System.Windows.Interop;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class HotKeyHost_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void AddNullKeyTest()
		{
			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					host.AddHotKey( new HotKey( (System.Windows.Input.Key)0, System.Windows.Input.ModifierKeys.None ) );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void AddNullTest()
		{
			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					host.AddHotKey( null );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" )]
		public void AddRemoveTest()
		{
			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					HotKey key = new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows );
					host.AddHotKey( key );
					CollectionAssert.Contains( host.HotKeys.ToArray(), key );

					bool raised = false;
					host.HotKeyPressed += ( s, e ) => raised = true;

					IntPtr wParam = new IntPtr( 2 ); // I have no idea why this is a two...
					INativeMethods.Instance.SendMessage_Wrapper( hwndSource.Handle, HotKeyHost.WM_HotKey, wParam, IntPtr.Zero );
					Assert.IsTrue( raised );

					host.RemoveHotKey( key );
					CollectionAssert.DoesNotContain( host.HotKeys.ToArray(), key );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" ), ExpectedException( typeof( HotKeyAlreadyRegisteredException ) )]
		public void AlreadyRegisteredTest()
		{
			SetNativeMethods( NativeMethodsType.Real );

			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					host.AddHotKey( new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows ) );
					host.AddHotKey( new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows ) );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void NullConstructorTest()
		{
			HotKeyHost host = new HotKeyHost( null );
		}
	}
}
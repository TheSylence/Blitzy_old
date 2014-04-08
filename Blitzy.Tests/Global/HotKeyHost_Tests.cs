// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	public class HotKeyHost_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" ), ExpectedException( typeof( ArgumentNullException ) )]
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

		[TestMethod, TestCategory( "Global" ), ExpectedException( typeof( ArgumentNullException ) )]
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

		[TestMethod, TestCategory( "Global" )]
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
					NativeMethods.SendMessage( hwndSource.Handle, HotKeyHost.WM_HotKey, wParam, IntPtr.Zero );
					Assert.IsTrue( raised );

					host.RemoveHotKey( key );
					CollectionAssert.DoesNotContain( host.HotKeys.ToArray(), key );
				}
			}
		}

		[TestMethod, TestCategory( "Global" ), ExpectedException( typeof( HotKeyAlreadyRegisteredException ) )]
		public void AlreadyRegisteredTest()
		{
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

		[TestMethod, TestCategory( "Global" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void NullConstructorTest()
		{
			HotKeyHost host = new HotKeyHost( null );
		}
	}
}
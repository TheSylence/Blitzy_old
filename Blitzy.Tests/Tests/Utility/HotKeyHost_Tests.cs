using System;
using System.Linq;
using System.Windows.Interop;
using Blitzy.Utility;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class HotKeyHost_Tests : TestBase
	{
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
					INativeMethods.Instance.SendMessage_Wrapper( hwndSource.Handle, HotKeyHost.WM_HOTKEY, wParam, IntPtr.Zero );
					Assert.IsTrue( raised );

					host.RemoveHotKey( key );
					CollectionAssert.DoesNotContain( host.HotKeys.ToArray(), key );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" )]
		public void AddTest()
		{
			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					ExceptionAssert.Throws<ArgumentNullException>( () => host.AddHotKey( new HotKey( (System.Windows.Input.Key)0, System.Windows.Input.ModifierKeys.None ) ) );

					ExceptionAssert.Throws<ArgumentNullException>( () => host.AddHotKey( null ) );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" )]
		public void AlreadyRegisteredTest()
		{
			SetNativeMethods( NativeMethodsType.Real );

			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					host.AddHotKey( new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows ) );
					ExceptionAssert.Throws<HotKeyAlreadyRegisteredException>( () => host.AddHotKey( new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows ) ) );
				}
			}
		}

		[TestMethod, TestCategory( "Utility" )]
		public void ConstructorTest()
		{
			ExceptionAssert.Throws<ArgumentNullException>( () => new HotKeyHost( null ) );
		}

		[TestMethod, TestCategory( "Utility" )]
		public void PropertyChangedTest()
		{
			HwndSourceParameters p = new HwndSourceParameters();

			using( HwndSource hwndSource = new HwndSource( p ) )
			{
				using( HotKeyHost host = new HotKeyHost( hwndSource ) )
				{
					HotKey key = new HotKey( System.Windows.Input.Key.A, System.Windows.Input.ModifierKeys.Windows );
					host.AddHotKey( key );

					using( ShimsContext.Create() )
					{
						bool registered = false;
						Blitzy.Utility.Fakes.ShimHotKeyHost.AllInstances.RegisterHotKeyInt32HotKey = ( h, i, k ) => registered = true;
						bool unregistered = false;
						Blitzy.Utility.Fakes.ShimHotKeyHost.AllInstances.UnregisterHotKeyInt32 = ( h, k ) => unregistered = true;

						key.Enabled = false;
						Assert.IsTrue( unregistered );

						key.Enabled = true;
						Assert.IsTrue( registered );

						unregistered = registered = false;
						key.Modifiers = System.Windows.Input.ModifierKeys.Control;
						Assert.IsTrue( unregistered );
						Assert.IsTrue( registered );

						unregistered = registered = false;
						key.Key = System.Windows.Input.Key.S;
						Assert.IsTrue( unregistered );
						Assert.IsTrue( registered );
					}
				}
			}
		}
	}
}
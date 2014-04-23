// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class HotKey_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void ConstructorTest()
		{
			HotKey key = new HotKey( System.Windows.Input.Key.Escape, System.Windows.Input.ModifierKeys.Control );
			Assert.AreEqual( System.Windows.Input.Key.Escape, key.Key );
			Assert.AreEqual( System.Windows.Input.ModifierKeys.Control, key.Modifiers );
			Assert.IsTrue( key.Enabled );
			Assert.AreEqual( "Control + Escape (Enabled)", key.ToString() );

			key = new HotKey( System.Windows.Input.Key.Escape, System.Windows.Input.ModifierKeys.Control, false );
			Assert.AreEqual( System.Windows.Input.Key.Escape, key.Key );
			Assert.AreEqual( System.Windows.Input.ModifierKeys.Control, key.Modifiers );
			Assert.IsFalse( key.Enabled );

			Assert.AreEqual( "Control + Escape (Not Enabled)", key.ToString() );
		}

		[TestMethod, TestCategory( "Global" )]
		public void HotKeyAlreadyRegisteredExceptionTest()
		{
			HotKey key = new HotKey();
			HotKeyAlreadyRegisteredException hex = new HotKeyAlreadyRegisteredException( "test", key );

			Assert.AreSame( key, hex.HotKey );
			Assert.AreEqual( "test", hex.Message );

			Exception ex = new Exception();
			hex = new HotKeyAlreadyRegisteredException( "test", key, ex );

			Assert.AreSame( key, hex.HotKey );
			Assert.AreEqual( "test", hex.Message );
			Assert.AreSame( ex, hex.InnerException );
		}

		[TestMethod, TestCategory( "Global" )]
		public void HotKeyEventArgsTest()
		{
			HotKey key = new HotKey();
			HotKeyEventArgs args = new HotKeyEventArgs( key );

			Assert.AreSame( key, args.HotKey );
		}

		[TestMethod, TestCategory( "Global" )]
		public void PropertyChangedTest()
		{
			HotKey key = new HotKey();
			PropertyChangedListener listener = new PropertyChangedListener( key );

			Assert.IsTrue( listener.TestProperties() );
		}

		[TestMethod, TestCategory( "Global" )]
		public void RaiseTest()
		{
			HotKey key = new HotKey( System.Windows.Input.Key.Escape, System.Windows.Input.ModifierKeys.Control );
			bool raised = false;
			key.HotKeyPressed += ( s, e ) => raised = true;

			key.RaiseOnHotKeyPressed();
			Assert.IsTrue( raised );
		}
	}
}
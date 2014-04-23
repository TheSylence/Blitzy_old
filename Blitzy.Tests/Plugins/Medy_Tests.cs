// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin.System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Medy_Tests : PluginTestBase
	{
		[TestMethod, TestCategory( "Plugins" )]
		public void ExecuteCommandTest()
		{
			int received_message = -1;
			long received_lparam = -1;
			SetNativeMethods( NativeMethodsType.Test );
			NativeMethods.OnSendMessage = ( hwnd, msg, wp, lp ) =>
			{
				received_message = msg;
				received_lparam = lp.ToInt64();

				return IntPtr.Zero;
			};

			Dictionary<string, long> expectedLparams = new Dictionary<string, long>();
			expectedLparams.Add( "play", Medy.APPCOMMAND_MEDIA_PLAY );
			expectedLparams.Add( "pause", Medy.APPCOMMAND_MEDIA_PAUSE );
			expectedLparams.Add( "next", Medy.APPCOMMAND_MEDIA_NEXTTRACK );
			expectedLparams.Add( "prev", Medy.APPCOMMAND_MEDIA_PREVIOUSTRACK );
			expectedLparams.Add( "voldn", Medy.APPCOMMAND_VOLUME_DOWN );
			expectedLparams.Add( "volup", Medy.APPCOMMAND_VOLUME_UP );

			Medy plug = new Medy();
			plug.Load( this );

			CommandItem command = plug.GetCommands( new List<string>() ).FirstOrDefault();
			Assert.IsNotNull( command );
			IEnumerable<CommandItem> commands = plug.GetSubCommands( command, new List<string>() );
			Assert.AreNotEqual( 0, commands.Count() );

			foreach( CommandItem cmd in commands )
			{
				string message;
				plug.ExecuteCommand( cmd, new List<string>(), out message );

				Assert.IsNull( message );
				Assert.AreEqual( Medy.WM_APPCOMMAND, received_message );
				Assert.AreEqual( expectedLparams[cmd.Name] * 65536L, received_lparam );
			}
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void InterfaceTest()
		{
			PluginTester tester = new PluginTester( new Medy() );
			Assert.IsTrue( tester.TestRunComplete );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void LoadUnloadTest()
		{
			Medy plug = new Medy();
			Assert.IsTrue( plug.Load( this ) );
			plug.Unload( Plugin.PluginUnloadReason.Unload );
		}
	}
}
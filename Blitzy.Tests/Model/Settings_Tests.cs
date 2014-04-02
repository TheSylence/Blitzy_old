// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	public class Settings_Tests : TestBase
	{
		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidGetKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( Connection );
			cfg.GetValue<string>( plug, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidGetPluginTest()
		{
			ISettings cfg = new Settings( Connection );
			cfg.GetValue<string>( null, "test" );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidRemoveKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( Connection );
			cfg.RemoveValue( plug, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidRemovePluginTest()
		{
			ISettings cfg = new Settings( Connection );
			cfg.RemoveValue( null, "test" );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidSetKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( Connection );
			cfg.SetValue( plug, null, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidSetPluginTest()
		{
			ISettings cfg = new Settings( Connection );
			cfg.SetValue( null, "test", null );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void ISettings_SetGetValueTest()
		{
			MockPlugin plug = new MockPlugin();
			ISettings cfg = new Settings( Connection );

			const int expected = 123;
			cfg.SetValue( plug, "test", expected );
			Assert.AreEqual( expected, cfg.GetValue<int>( plug, "test" ) );
		}
	}
}
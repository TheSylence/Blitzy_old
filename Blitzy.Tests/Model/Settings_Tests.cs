// $Id$

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
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
		[TestMethod, TestCategory( "Model" )]
		public void DefaultsTest()
		{
			Settings cfg = new Settings( Connection );
			cfg.SetDefaults();

			Type type = typeof( SystemSetting );

			foreach( SystemSetting setting in Enum.GetValues( type ) )
			{
				MemberInfo member = type.GetMember( setting.ToString() ).First();
				object defaultValue = member.GetCustomAttribute<DefaultValueAttribute>().Value;

				Assert.AreEqual( defaultValue.ToString(), cfg.GetValue<string>( setting ) );
			}
		}

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

		[TestMethod, TestCategory( "Plugins" )]
		public void ISettings_SystemSettingsTest()
		{
			ISettings cfg = new Settings( Connection );

			Assert.AreEqual( ( (Settings)cfg ).GetValue<int>( SystemSetting.MaxMatchingItems ), cfg.GetSystemSetting<int>( SystemSetting.MaxMatchingItems ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			Settings cfg = new Settings( Connection );
			cfg.Folders.Add( new Folder() { ID = 123, IsRecursive = true, Path = "C:\\temp" } );
			cfg.Folders.Add( new Folder() { ID = 456, IsRecursive = false, Path = "C:\\temp2" } );

			cfg.Save();

			cfg = new Settings( Connection );
			cfg.Load();

			Assert.AreEqual( 2, cfg.Folders.Count );
			Folder folder = cfg.Folders.Where( f => f.ID == 123 ).FirstOrDefault();
			Assert.IsNotNull( folder );
			Assert.AreEqual( true, folder.IsRecursive );
			Assert.AreEqual( "C:\\temp", folder.Path );

			folder = cfg.Folders.Where( f => f.ID == 456 ).FirstOrDefault();
			Assert.IsNotNull( folder );
			Assert.AreEqual( false, folder.IsRecursive );
			Assert.AreEqual( "C:\\temp2", folder.Path );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SystemSettingTest()
		{
			Settings cfg = new Settings( Connection );
			cfg.SetDefaults();

			cfg.SetValue( SystemSetting.Language, "test" );
			Assert.AreEqual( "test", cfg.GetValue<string>( SystemSetting.Language ) );

			cfg.SetValue( SystemSetting.MaxMatchingItems, 123 );
			Assert.AreEqual( 123, cfg.GetValue<int>( SystemSetting.MaxMatchingItems ) );

			cfg.SetValue( SystemSetting.CloseOnEscape, false );
			Assert.AreEqual( false, cfg.GetValue<bool>( SystemSetting.CloseOnEscape ) );

			cfg.SetValue( SystemSetting.CloseOnEscape, true );
			Assert.AreEqual( true, cfg.GetValue<bool>( SystemSetting.CloseOnEscape ) );
		}
	}
}
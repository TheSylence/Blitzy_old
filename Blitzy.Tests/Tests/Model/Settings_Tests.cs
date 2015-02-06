using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Tests.Mocks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Settings_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void ConvertValueTest()
		{
			Settings cfg = new Settings( ConnectionFactory );

			Assert.AreEqual( 0L, cfg.ConvertValue<long>( null ) );
			Assert.AreEqual( 0L, cfg.ConvertValue<long>( DBNull.Value ) );

			Assert.AreEqual( true, cfg.ConvertValue<bool>( 1 ) );
			Assert.AreEqual( false, cfg.ConvertValue<bool>( 0 ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void DefaultsTest()
		{
			Settings cfg = new Settings( ConnectionFactory );
			cfg.SetDefaults();

			Type type = typeof( SystemSetting );

			foreach( SystemSetting setting in Enum.GetValues( type ) )
			{
				MemberInfo member = type.GetMember( setting.ToString() ).First();
				object defaultValue = member.GetCustomAttribute<DefaultValueAttribute>().Value;

				Assert.AreEqual( defaultValue.ToString(), cfg.GetValue<string>( setting ) );
			}
		}

		[TestMethod, TestCategory( "Model" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void InvalidGetPluginSettingTest()
		{
			Settings cfg = new Settings( ConnectionFactory );
			cfg.GetPluginSetting<string>( null, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidGetKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( ConnectionFactory );
			cfg.GetValue<string>( plug, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidGetPluginTest()
		{
			ISettings cfg = new Settings( ConnectionFactory );
			cfg.GetValue<string>( null, "test" );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidRemoveKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( ConnectionFactory );
			cfg.RemoveValue( plug, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidRemovePluginTest()
		{
			ISettings cfg = new Settings( ConnectionFactory );
			cfg.RemoveValue( null, "test" );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidSetKeyTest()
		{
			MockPlugin plug = new MockPlugin();

			ISettings cfg = new Settings( ConnectionFactory );
			cfg.SetValue( plug, null, null );
		}

		[TestMethod, TestCategory( "Plugins" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void ISettings_InvalidSetPluginTest()
		{
			ISettings cfg = new Settings( ConnectionFactory );
			cfg.SetValue( null, "test", null );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void ISettings_RemovePluginTest()
		{
			MockPlugin plug = new MockPlugin();
			ISettings cfg = new Settings( ConnectionFactory );

			cfg.RemoveValue( plug, "test" );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void ISettings_SetGetValueTest()
		{
			MockPlugin plug = new MockPlugin();
			ISettings cfg = new Settings( ConnectionFactory );

			const int expected = 123;
			cfg.SetValue( plug, "test", expected );
			Assert.AreEqual( expected, cfg.GetValue<int>( plug, "test" ) );
		}

		[TestMethod, TestCategory( "Plugins" )]
		public void ISettings_SystemSettingsTest()
		{
			ISettings cfg = new Settings( ConnectionFactory );

			Assert.AreEqual( ( (Settings)cfg ).GetValue<int>( SystemSetting.MaxMatchingItems ), cfg.GetSystemSetting<int>( SystemSetting.MaxMatchingItems ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void SaveLoadTest()
		{
			int id1 = TestHelper.NextID();
			int id2 = TestHelper.NextID();

			Settings cfg = new Settings( ConnectionFactory );
			using( Folder f1 = new Folder() { ID = id1, IsRecursive = true, Path = "C:\\temp" } )
			{
				cfg.Folders.Add( f1 );
				using( Folder f2 = new Folder() { ID = id2, IsRecursive = false, Path = "C:\\temp2" } )
				{
					cfg.Folders.Add( f2 );

					cfg.Save();

					cfg = new Settings( ConnectionFactory );
					cfg.Load();

					Assert.IsTrue( cfg.Folders.Count >= 4 ); // 2 Start menu entries + 2 tested ones
					Folder folder = cfg.Folders.Where( f => f.ID == id1 ).FirstOrDefault();
					Assert.IsNotNull( folder );
					Assert.AreEqual( true, folder.IsRecursive );
					Assert.AreEqual( "C:\\temp", folder.Path );

					folder = cfg.Folders.Where( f => f.ID == id2 ).FirstOrDefault();
					Assert.IsNotNull( folder );
					Assert.AreEqual( false, folder.IsRecursive );
					Assert.AreEqual( "C:\\temp2", folder.Path );
				}
			}
		}

		[TestMethod, TestCategory( "Model" )]
		public void SystemSettingTest()
		{
			Settings cfg = new Settings( ConnectionFactory );
			cfg.SetDefaults();
			try
			{
				cfg.SetValue( SystemSetting.Language, "test" );
				Assert.AreEqual( "test", cfg.GetValue<string>( SystemSetting.Language ) );

				cfg.SetValue( SystemSetting.MaxMatchingItems, 123 );
				Assert.AreEqual( 123, cfg.GetValue<int>( SystemSetting.MaxMatchingItems ) );

				cfg.SetValue( SystemSetting.CloseOnEscape, false );
				Assert.AreEqual( false, cfg.GetValue<bool>( SystemSetting.CloseOnEscape ) );

				cfg.SetValue( SystemSetting.CloseOnEscape, true );
				Assert.AreEqual( true, cfg.GetValue<bool>( SystemSetting.CloseOnEscape ) );
			}
			finally
			{
				cfg.SetDefaults();
			}
		}
	}
}
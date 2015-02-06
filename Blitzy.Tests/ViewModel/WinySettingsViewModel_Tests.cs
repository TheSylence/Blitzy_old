using Blitzy.Plugin.SystemPlugins;
using Blitzy.Tests.Mocks;
using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WinySettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void LoadSaveTest()
		{
			SettingsViewModel baseVM = new SettingsViewModel( ConnectionFactory );
			baseVM.Settings = new Blitzy.Model.Settings( ConnectionFactory );
			MockPluginHost host = new MockPluginHost( baseVM.Settings );
			baseVM.PluginManager = new Plugin.PluginManager( host, ConnectionFactory );
			baseVM.PluginManager.LoadPlugins();

			Winy winy = new Winy();
			winy.SetDefaultSettings( baseVM.Settings );

			baseVM.Reset();

			Assert.IsTrue( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).LogoffConfirmation );
			Assert.IsTrue( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).RestartConfirmation );
			Assert.IsTrue( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).ShutdownConfirmation );

			baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).LogoffConfirmation = false;
			baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).RestartConfirmation = false;
			baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).ShutdownConfirmation = false;

			baseVM.SaveCommand.Execute( null );

			baseVM.Reset();

			Assert.IsFalse( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).LogoffConfirmation );
			Assert.IsFalse( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).RestartConfirmation );
			Assert.IsFalse( baseVM.GetPluginContext<WinySettingsViewModel>( "Winy" ).ShutdownConfirmation );
		}
	}
}
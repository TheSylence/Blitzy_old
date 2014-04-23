// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin.System;
using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class WinySettingsViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void LoadSaveTest()
		{
			SettingsViewModel basevm = new SettingsViewModel();
			basevm.Settings = new Blitzy.Model.Settings( Connection );

			Winy winy = new Winy();
			winy.SetDefaultSettings( basevm.Settings );

			basevm.Reset();

			Assert.IsTrue( basevm.WinySettings.LogoffConfirmation );
			Assert.IsTrue( basevm.WinySettings.RestartConfirmation );
			Assert.IsTrue( basevm.WinySettings.ShutdownConfirmation );

			basevm.WinySettings.LogoffConfirmation = false;
			basevm.WinySettings.RestartConfirmation = false;
			basevm.WinySettings.ShutdownConfirmation = false;

			basevm.SaveCommand.Execute( null );

			basevm.Reset();

			Assert.IsFalse( basevm.WinySettings.LogoffConfirmation );
			Assert.IsFalse( basevm.WinySettings.RestartConfirmation );
			Assert.IsFalse( basevm.WinySettings.ShutdownConfirmation );
		}
	}
}
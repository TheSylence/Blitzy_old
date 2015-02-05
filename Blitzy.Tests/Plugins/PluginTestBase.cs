using System;
using Blitzy.Model;
using Blitzy.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Plugins
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class PluginTestBase : TestBase, IPluginHost
	{
		[TestInitialize]
		public override void BeforeTestRun()
		{
			base.BeforeTestRun();

			APIDatabase = new PluginDatabase( Connection );
			Settings = new Settings( Connection );
		}

		bool IPluginHost.IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}

		IDatabase IPluginHost.Database
		{
			get { return APIDatabase; }
		}

		ISettings IPluginHost.Settings
		{
			get { return Settings; }
		}

		private PluginDatabase APIDatabase;
		private Settings Settings;
	}
}
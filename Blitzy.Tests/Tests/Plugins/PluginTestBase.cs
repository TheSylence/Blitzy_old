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

			Settings = new Settings( ConnectionFactory );
		}

		bool IPluginHost.IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}

		DbConnectionFactory IPluginHost.ConnectionFactory
		{
			get { return ConnectionFactory; }
		}

		ISettings IPluginHost.Settings
		{
			get { return Settings; }
		}

		private Settings Settings;
	}
}
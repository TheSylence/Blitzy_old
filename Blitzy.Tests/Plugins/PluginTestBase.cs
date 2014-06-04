// $Id$

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

		protected override void CreatePluginTables()
		{
			// Do nothing
		}

		#region IPluginHost

		IDatabase IPluginHost.Database
		{
			get { return APIDatabase; }
		}

		ISettings IPluginHost.Settings
		{
			get { return Settings; }
		}

		bool IPluginHost.IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}

		#endregion IPluginHost

		#region Properties

		private PluginDatabase APIDatabase;
		private Settings Settings;

		#endregion Properties
	}
}
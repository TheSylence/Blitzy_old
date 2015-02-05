

using System.Diagnostics.CodeAnalysis;
using Blitzy.Model;
using Blitzy.Plugin;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SettingsServiceParameters
	{
		public readonly CatalogBuilder Builder;
		public readonly PluginManager PluginManager;
		public readonly Settings Settings;

		public SettingsServiceParameters( Settings settings, CatalogBuilder builder, PluginManager pluginManager )
		{
			Settings = settings;
			Builder = builder;
			PluginManager = pluginManager;
		}
	}
}
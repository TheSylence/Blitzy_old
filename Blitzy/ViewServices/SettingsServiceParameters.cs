// $Id$

using System.Diagnostics.CodeAnalysis;
using Blitzy.Model;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	internal class SettingsServiceParameters
	{
		public readonly CatalogBuilder Builder;
		public readonly Settings Settings;

		public SettingsServiceParameters( Settings settings, CatalogBuilder builder )
		{
			Settings = settings;
			Builder = builder;
		}
	}
}
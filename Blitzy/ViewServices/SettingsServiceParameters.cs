// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
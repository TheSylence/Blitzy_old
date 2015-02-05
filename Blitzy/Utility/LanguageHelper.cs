

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Utility
{
	internal static class LanguageHelper
	{
		internal static IEnumerable<CultureInfo> GetAvailableLanguages()
		{
			return WPFLocalizeExtension.Providers.ResxLocalizationProvider.Instance.AvailableCultures.Where( c => !string.IsNullOrWhiteSpace( c.IetfLanguageTag ) );
		}

		internal static CultureInfo GetLanguage( string language )
		{
			CultureInfo culture = CultureInfo.CreateSpecificCulture( language );

			IEnumerable<CultureInfo> availableLanguages = GetAvailableLanguages();
			while( !availableLanguages.Contains( culture ) && culture != null )
			{
				if( string.IsNullOrWhiteSpace( culture.Parent.Name ) )
				{
					culture = null;
					break;
				}

				culture = culture.Parent;
			}

			return culture;
		}
	}
}
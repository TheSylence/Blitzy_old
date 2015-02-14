using System.Globalization;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class LanguageMessage : MessageBase
	{
		public LanguageMessage( CultureInfo language )
		{
			Language = language;
		}

		public readonly CultureInfo Language;
	}
}
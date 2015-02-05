

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class LanguageMessage : MessageBase
	{
		public readonly CultureInfo Language;

		public LanguageMessage( CultureInfo language )
		{
			Language = language;
		}
	}
}
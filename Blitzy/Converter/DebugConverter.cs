// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Blitzy.Converter
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class DebugConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			if( Debugger.IsAttached )
			{
				Debugger.Break();
			}

			return value;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			if( Debugger.IsAttached )
			{
				Debugger.Break();
			}

			return value;
		}
	}
}
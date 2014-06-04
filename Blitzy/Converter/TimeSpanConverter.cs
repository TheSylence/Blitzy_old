// $Id$

using System;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class TimeSpanConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return ( (TimeSpan)value ).ToString( "hh\\:mm\\:ss" );
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
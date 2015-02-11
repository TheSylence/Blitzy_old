using System;
using System.Globalization;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class TimeSpanConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			return ( (TimeSpan)value ).ToString( "hh\\:mm\\:ss" );
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
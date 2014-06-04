// $Id$

using System;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class ElementNotNullConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return value != null;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
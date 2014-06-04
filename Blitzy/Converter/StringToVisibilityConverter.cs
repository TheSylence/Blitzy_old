// $Id$

using System;
using System.Windows;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class StringToVisibilityConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			string str = value as string;

			if( string.IsNullOrWhiteSpace( str ) )
			{
				return Visibility.Collapsed;
			}

			return Visibility.Visible;
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
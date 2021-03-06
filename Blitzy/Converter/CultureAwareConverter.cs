﻿using System;
using System.Threading;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class CultureAwareConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			return System.Convert.ChangeType( value, targetType, Thread.CurrentThread.CurrentUICulture );
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
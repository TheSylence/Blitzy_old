﻿using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using Blitzy.Utility;

namespace Blitzy.Converter
{
	internal class LocalizeFormatterConverter : IMultiValueConverter
	{
		public object Convert( object[] values, Type targetType, object parameter, CultureInfo culture )
		{
			if( parameter == null )
			{
				return values;
			}

			object[] bindingParams = (object[])values.Clone();

			// Remove the {DependancyProperty.UnsetValue} from unbound datasources.
			for( int i = 0; i < bindingParams.Length; i++ )
			{
				if( bindingParams[i] != null && bindingParams[i] == DependencyProperty.UnsetValue )
				{
					bindingParams[i] = null;
				}
			}

			string keyStr = ( (string)parameter ).Localize();
			return string.Format( keyStr, bindingParams );
		}

		public object[] ConvertBack( object value, Type[] targetTypes, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
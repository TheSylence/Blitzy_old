﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class FileSizeConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			string[] suffixes = new string[]
			{
				"B",
				"KB",
				"MB",
				"GB",
				"TB",
				"PB"
			};

			int idx = 0;
			double size = System.Convert.ToDouble( value );
			while( size >= 1024 )
			{
				size /= 1024.0;
				++idx;
			}

			return string.Format( culture, "{0} {1}", Math.Round( size, 2 ), suffixes[idx] );
		}

		public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
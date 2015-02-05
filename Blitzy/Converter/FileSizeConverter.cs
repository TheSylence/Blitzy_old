

using System;
using System.Globalization;
using System.Windows.Data;

namespace Blitzy.Converter
{
	internal class FileSizeConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string[] suffixes =
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

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
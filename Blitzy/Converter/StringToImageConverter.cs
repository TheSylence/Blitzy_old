using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Blitzy.Model;

namespace Blitzy.Converter
{
	[SuppressMessage( "Microsoft.Performance", "CA1812", Justification = "Used in XAML" )]
	internal class StringToImageConverter : IValueConverter
	{
		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string str = value as string;
			if( str == null )
				return DependencyProperty.UnsetValue;

			if( str.Contains( ":" ) && !Uri.IsWellFormedUriString( str, UriKind.Absolute ) )
			{
				str = ShellLinkHelper.ResolveX64Path( str );

				if( Pattern.IsMatch( str ) )
				{
					int idx = str.LastIndexOf( ',' );
					string file = str.Substring( 0, idx );
					str = str.Substring( idx + 1 );

					// This seems to be never reached
					//if( str.Contains( "." ) ) // No icon but file name...
					//{
					//	return null;
					//}
					int icoIdx;
					icoIdx = int.Parse( str, CultureInfo.InvariantCulture );

					IntPtr large = IntPtr.Zero;
					IntPtr small = IntPtr.Zero;
					try
					{
						int icons = INativeMethods.Instance.ExtractIconEx_Wrapper( file, icoIdx, ref large, ref small, 1 );
						if( icons == 0 )
						{
							LogHelper.LogWarning( MethodBase.GetCurrentMethod().DeclaringType, "No icons extracted from {0}", file );
						}

						IntPtr ico = large;
						if( ico.Equals( IntPtr.Zero ) )
						{
							ico = small;
							if( ico.Equals( IntPtr.Zero ) )
								return DependencyProperty.UnsetValue;
						}

						using( Icon i = Icon.FromHandle( ico ) )
						{
							return Imaging.CreateBitmapSourceFromHIcon( i.Handle, new Int32Rect( 0, 0, i.Width, i.Height ), BitmapSizeOptions.FromEmptyOptions() );
						}
					}
					finally
					{
						INativeMethods.Instance.DestroyIcon_Wrapper( large );
						INativeMethods.Instance.DestroyIcon_Wrapper( small );
					}
				}

				if( File.Exists( str ) )
				{
					using( Icon i = Icon.ExtractAssociatedIcon( str ) )
					{
						if( i != null )
						{
							return Imaging.CreateBitmapSourceFromHIcon( i.Handle, new Int32Rect( 0, 0, i.Width, i.Height ),
								BitmapSizeOptions.FromEmptyOptions() );
						}
					}
				}

				return DependencyProperty.UnsetValue;
			}

			string uri = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), "icons", str );
			BitmapImage img;

			if( !File.Exists( uri ) )
			{
				Stream manifest = Assembly.GetExecutingAssembly().GetManifestResourceStream( string.Format( "Blitzy.Resources.Icons.{0}", str ) );
				if( manifest != null )
				{
					img = new BitmapImage();
					img.BeginInit();
					img.StreamSource = manifest;
					img.EndInit();
					return img;
				}

				if( Uri.IsWellFormedUriString( str, UriKind.Absolute ) )
				{
					img = new BitmapImage();
					img.BeginInit();
					img.UriSource = new Uri( str, UriKind.Absolute );
					img.EndInit();
					return img;
				}

				return DependencyProperty.UnsetValue;
			}

			img = new BitmapImage();
			img.BeginInit();
			img.UriSource = new Uri( uri, UriKind.Absolute );
			img.EndInit();
			return img;
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}

		private readonly Regex Pattern = new Regex( "^.*,[0-9-]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant );
	}
}
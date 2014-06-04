// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Blitzy.Model.Shell;

namespace Blitzy.Converter
{
	[SuppressMessage( "Microsoft.Performance", "CA1812", Justification = "Used in XAML" )]
	internal class StringToImageConverter : IValueConverter
	{
		private Regex Pattern = new Regex( "^.*,[0-9-]+$", RegexOptions.Compiled | RegexOptions.Singleline | RegexOptions.CultureInvariant );

		public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
		{
			string str = value as string;
			if( str == null )
				return null;

			if( str.Contains( ":" ) && !Uri.IsWellFormedUriString( str, UriKind.Absolute ) )
			{
				str = ShellLinkHelper.ResolveX64Path( str );

				if( Pattern.IsMatch( str ) )
				{
					int idx = str.LastIndexOf( ',' );
					string file = str.Substring( 0, idx );
					str = str.Substring( idx + 1 );

					if( str.Contains( "." ) ) // No icon but file name...
					{
						return null;
					}
					int icoIdx;
					try
					{
						icoIdx = int.Parse( str, CultureInfo.InvariantCulture );
					}
					catch( FormatException )
					{
						return null;
					}

					IntPtr large = IntPtr.Zero;
					IntPtr small = IntPtr.Zero;
					int icons = INativeMethods.Instance.ExtractIconEx_Wrapper( file, icoIdx, ref large, ref small, 1 );
					if( icons == 0 )
					{
						LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "No icons extracted from {0}", file );
					}

					IntPtr ico = large;
					if( ico.Equals( IntPtr.Zero ) )
					{
						ico = small;
						if( ico.Equals( IntPtr.Zero ) )
							return null;
					}

					using( System.Drawing.Icon i = System.Drawing.Icon.FromHandle( ico ) )
					{
						return Imaging.CreateBitmapSourceFromHIcon( i.Handle, new System.Windows.Int32Rect( 0, 0, i.Width, i.Height ), BitmapSizeOptions.FromEmptyOptions() );
					}
				}
				else
				{
					if( File.Exists( str ) )
					{
						using( System.Drawing.Icon i = System.Drawing.Icon.ExtractAssociatedIcon( str ) )
						{
							return Imaging.CreateBitmapSourceFromHIcon( i.Handle, new System.Windows.Int32Rect( 0, 0, i.Width, i.Height ), BitmapSizeOptions.FromEmptyOptions() );
						}
					}

					return null;
				}
			}
			else
			{
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

					return null;
				}

				img = new BitmapImage();
				img.BeginInit();
				img.UriSource = new Uri( uri, UriKind.Absolute );
				img.EndInit();
				return img;
			}
		}

		public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
		{
			throw new NotSupportedException();
		}
	}
}
using System.Windows;
using System.Windows.Data;

namespace Blitzy.View
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class CultureAwareBinding : Binding
	{
		public CultureAwareBinding( string path )
			: base( path )
		{
			Converter = (IValueConverter)Application.Current.FindResource( "CultureAwareConv" );
			//ConverterCulture = Thread.CurrentThread.CurrentUICulture;
		}
	}
}
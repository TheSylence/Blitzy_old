using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace Blitzy.Behaviors
{
	[ExcludeFromCodeCoverage]
	public class HtmlBehavior : Behavior<WebBrowser>
	{
		[AttachedPropertyBrowsableForType( typeof( WebBrowser ) )]
		public static string GetHtml( WebBrowser d )
		{
			return (string)d.GetValue( HtmlProperty );
		}

		public static void SetHtml( WebBrowser d, string value )
		{
			d.SetValue( HtmlProperty, value );
		}

		private static void OnHtmlChanged( DependencyObject d, DependencyPropertyChangedEventArgs e )
		{
			WebBrowser wb = d as WebBrowser;
			if( wb != null )
				wb.NavigateToString( e.NewValue as string );
		}

		public static readonly DependencyProperty HtmlProperty = DependencyProperty.RegisterAttached( "Html", typeof( string ),
			typeof( HtmlBehavior ), new FrameworkPropertyMetadata( OnHtmlChanged ) );
	}
}
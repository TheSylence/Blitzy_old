

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using Blitzy.Messages;
using GalaSoft.MvvmLight.Messaging;

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
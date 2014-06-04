// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class LocalizeFormatterConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			LocalizeFormatterConverter conv = new LocalizeFormatterConverter();

			conv.ConvertBack( true, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			LocalizeFormatterConverter conv = new LocalizeFormatterConverter();

			object[] values = new object[]
			{
				12,
				150,
				122
			};
			string result = conv.Convert( values, typeof( string ), "DownloadProgress", CultureInfo.InvariantCulture ) as string;
			string expected = "Downloading 12 of 150 - Estimated time left: 122";

			Assert.AreEqual( expected, result );

			Assert.IsNull( conv.Convert( null, null, null, null ) );

			values = new object[]
			{
				null,
				DependencyProperty.UnsetValue,
				null
			};

			result = conv.Convert( values, typeof( string ), "DownloadProgress", CultureInfo.InvariantCulture ) as string;
			expected = "Downloading  of  - Estimated time left: ";
			Assert.AreEqual( expected, result );
		}
	}
}
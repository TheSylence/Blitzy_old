// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class FileSizeConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			FileSizeConverter conv = new FileSizeConverter();

			conv.ConvertBack( true, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			FileSizeConverter conv = new FileSizeConverter();

			Dictionary<double, string> tests = new Dictionary<double, string>();
			tests.Add( 123.44, "123.44 B" );
			tests.Add( 2048, "2 KB" );
			tests.Add( 2560, "2.5 KB" );
			tests.Add( 2932342, "2.8 MB" );
			tests.Add( 293234823042, "273.1 GB" );

			foreach( KeyValuePair<double, string> kvp in tests )
			{
				Assert.AreEqual( kvp.Value, conv.Convert( kvp.Key, null, null, CultureInfo.InvariantCulture ) );
			}
		}
	}
}
// $Id$

using System;
using System.Globalization;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class TimeSpanConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			TimeSpanConverter conv = new TimeSpanConverter();

			conv.ConvertBack( true, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			TimeSpan span = new TimeSpan( 0, 0, 0, 1, 12 );
			const string expected = "00:00:01";

			TimeSpanConverter conv = new TimeSpanConverter();
			Assert.AreEqual( expected, conv.Convert( span, null, null, CultureInfo.InvariantCulture ) );
		}
	}
}
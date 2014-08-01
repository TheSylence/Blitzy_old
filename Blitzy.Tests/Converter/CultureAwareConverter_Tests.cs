// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	public class CultureAwareConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			CultureAwareConverter conv = new CultureAwareConverter();
			conv.ConvertBack( null, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			CultureAwareConverter conv = new CultureAwareConverter();

			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture( "de-DE" );
			string actual = (string)conv.Convert( 1.23f, typeof( string ), null, null );
			string expected = "1,23";
			Assert.AreEqual( expected, actual );

			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture( "en-US" );
			actual = (string)conv.Convert( 1.23f, typeof( string ), null, null );
			expected = "1.23";
			Assert.AreEqual( expected, actual );
		}
	}
}
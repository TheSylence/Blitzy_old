// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	public class StringToImageConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			StringToImageConverter conv = new StringToImageConverter();

			conv.ConvertBack( null, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			StringToImageConverter conv = new StringToImageConverter();

			Assert.IsNull( conv.Convert( 123, null, null, null ), "No string" );

			Assert.IsNotNull( conv.Convert( "Quit.png", null, null, null ), "Embedded Resource" );
			Assert.IsNull( conv.Convert( "Quit.png.Invalid", null, null, null ), "Invalid Embedded Resource" );

			string fileName = "../TestData/Gears.png";
			Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "Relative file name" );

			fileName = Path.Combine( Directory.GetCurrentDirectory(), "Blitzy.exe" );
			fileName += ",0";

			Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "Absolute file name" );

			fileName += "_1";
			Assert.IsNull( conv.Convert( fileName, null, null, null ), "Absolute file name with wrong icon index" );
		}
	}
}
// $Id$

using System;
using System.IO;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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

			fileName = "https://www.google.de/images/google_favicon_128.png";
			Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "URL File" );

			fileName = Path.Combine( Directory.GetCurrentDirectory(), "Blitzy.exe" );
			fileName += ",0,0";
			Assert.IsNull( conv.Convert( fileName, null, null, null ), "Double coma at end" );

			fileName = "C:\\temp\test,file.png,0";
			Assert.IsNull( conv.Convert( fileName, null, null, null ), "Coma in file name" );
		}
	}
}
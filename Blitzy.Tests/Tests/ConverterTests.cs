using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests
{
	[TestClass, System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ConverterTests : TestBase
	{
		[TestInitialize]
		public override void BeforeTestRun()
		{
			base.BeforeTestRun();
		}

		[TestMethod, TestCategory( "Converter" )]
		public void CultureAwareConverter()
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

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( null, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ElementNotNullConverter()
		{
			ElementNotNullConverter conv = new ElementNotNullConverter();

			Assert.IsTrue( (bool)conv.Convert( string.Empty, null, null, null ) );
			Assert.IsFalse( (bool)conv.Convert( null, null, null, null ) );

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void FileSizeConverter()
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

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void LocalizeFormatterConverter()
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

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void StringToImageConverter()
		{
			// FIXME: The PInvoke call to ExtractIconEx causes an AppDomainUnloadedException when running this test
			//StringToImageConverter conv = new StringToImageConverter();

			//Assert.IsNull( conv.Convert( 123, null, null, null ), "No string" );

			//Assert.IsNotNull( conv.Convert( "Quit.png", null, null, null ), "Embedded Resource" );
			//Assert.IsNull( conv.Convert( "Quit.png.Invalid", null, null, null ), "Invalid Embedded Resource" );

			//string fileName = "../TestData/Gears.png";
			//Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "Relative file name" );

			//fileName = Path.Combine( Directory.GetCurrentDirectory(), "Blitzy.exe" );
			//Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "Absolute file name" );

			//fileName += ",0";
			//Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "Absolute file name with index" );

			//fileName += "_1";
			//Assert.IsNull( conv.Convert( fileName, null, null, null ), "Absolute file name with wrong icon index" );

			//fileName = "https://www.google.de/images/google_favicon_128.png";
			//Assert.IsNotNull( conv.Convert( fileName, null, null, null ), "URL File" );

			//fileName = Path.Combine( Directory.GetCurrentDirectory(), "Blitzy.exe" );
			//fileName += ",0,0";
			//Assert.IsNull( conv.Convert( fileName, null, null, null ), "Double coma at end" );

			//fileName = "C:\\temp\test,file.png,0";
			//Assert.IsNull( conv.Convert( fileName, null, null, null ), "Coma in file name" );

			//fileName = "C:\\temp\\test.png,0.ext";
			//Assert.IsNull( conv.Convert( fileName, null, null, null ), "File name as icon" );

			//ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void StringToVisibilityConverter()
		{
			StringToVisibilityConverter conv = new StringToVisibilityConverter();

			Assert.AreEqual( Visibility.Collapsed, conv.Convert( null, null, null, null ) );
			Assert.AreEqual( Visibility.Collapsed, conv.Convert( string.Empty, null, null, null ) );
			Assert.AreEqual( Visibility.Collapsed, conv.Convert( "  \t", null, null, null ) );
			Assert.AreEqual( Visibility.Visible, conv.Convert( "test", null, null, null ) );

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void TimeSpanConverter()
		{
			TimeSpan span = new TimeSpan( 0, 0, 0, 1, 12 );
			const string expected = "00:00:01";

			TimeSpanConverter conv = new TimeSpanConverter();
			Assert.AreEqual( expected, conv.Convert( span, null, null, CultureInfo.InvariantCulture ) );

			ExceptionAssert.Throws<NotSupportedException>( () => conv.ConvertBack( true, null, null, null ) );
		}
	}
}
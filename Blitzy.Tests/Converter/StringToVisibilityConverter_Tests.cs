﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	public class StringToVisibilityConverter_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			StringToVisibilityConverter conv = new StringToVisibilityConverter();

			conv.ConvertBack( null, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			StringToVisibilityConverter conv = new StringToVisibilityConverter();

			Assert.AreEqual( Visibility.Collapsed, conv.Convert( null, null, null, null ) );
			Assert.AreEqual( Visibility.Collapsed, conv.Convert( string.Empty, null, null, null ) );
			Assert.AreEqual( Visibility.Collapsed, conv.Convert( "  \t", null, null, null ) );
			Assert.AreEqual( Visibility.Visible, conv.Convert( "test", null, null, null ) );
		}
	}
}
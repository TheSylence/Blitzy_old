﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Converter;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Converter
{
	[TestClass]
	public class ElementNotNull_Tests : TestBase
	{
		[TestMethod, TestCategory( "Converter" ), ExpectedException( typeof( NotSupportedException ) )]
		public void ConvertBackTest()
		{
			ElementNotNullConverter conv = new ElementNotNullConverter();

			conv.ConvertBack( true, null, null, null );
		}

		[TestMethod, TestCategory( "Converter" )]
		public void ConvertTest()
		{
			ElementNotNullConverter conv = new ElementNotNullConverter();

			Assert.IsTrue( (bool)conv.Convert( string.Empty, null, null, null ) );
			Assert.IsFalse( (bool)conv.Convert( null, null, null, null ) );
		}
	}
}
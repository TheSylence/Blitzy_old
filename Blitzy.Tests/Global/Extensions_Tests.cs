﻿// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Extensions_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void DiceCoefficentTest()
		{
			string a = "work";
			string b = "wirk";

			Assert.AreEqual( 0.5, a.GetDiceCoefficent( b ) );

			a = "abcd";
			b = "0123";
			Assert.AreEqual( 0.0, a.GetDiceCoefficent( b ) );

			a = b;
			Assert.AreEqual( 1.0, a.GetDiceCoefficent( b ) );
		}

		[TestMethod, TestCategory( "Global" ), ExpectedException( typeof( ArgumentNullException ) )]
		public void LocalizeNullTest()
		{
			string str = null;

			str.Localize();
		}

		[TestMethod, TestCategory( "Global" )]
		public void LocalizeTest()
		{
			string str = "Cancel";

			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture( "de" );
			Assert.AreEqual( "Abbrechen", str.Localize() );

			Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture( "en" );
			Assert.AreEqual( "Cancel", str.Localize() );

			Assert.AreEqual( "p.Cancel", str.Localize( "p." ) );
			Assert.AreEqual( "p.Cancel.s", str.Localize( "p.", ".s" ) );
			Assert.AreEqual( "Cancel.s", str.Localize( null, ".s" ) );
		}
	}
}
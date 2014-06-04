// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ShuntingYard_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void InvalidTests()
		{
			ShuntingYard calc = new ShuntingYard();
			Assert.AreEqual( "EmptyExpression".Localize(), calc.Calculate( "" ) );
			Assert.AreEqual( "EmptyExpression".Localize(), calc.Calculate( "()" ) );

			Assert.AreEqual( "SyntaxError".Localize(), calc.Calculate( "(3+" ) );
			Assert.AreEqual( "SyntaxError".Localize(), calc.Calculate( "3+5)" ) );

			Assert.AreEqual( "IncompleteExpression".Localize(), calc.Calculate( "log(2)" ) );
			Assert.AreEqual( "IncompleteExpression".Localize(), calc.Calculate( "3+" ) );
		}

		[TestMethod, TestCategory( "Model" )]
		public void OperatorsTest()
		{
			ShuntingYard calc = new ShuntingYard();

			Assert.AreEqual( "8", calc.Calculate( "3+5" ), "3+5" );
			Assert.AreEqual( "12", calc.Calculate( "4*3" ), "4*3" );
			Assert.AreEqual( "5", calc.Calculate( "8-3" ), "8-3" );
			Assert.AreEqual( "2", calc.Calculate( "8/4" ), "8/4" );
			Assert.AreEqual( "13", calc.Calculate( "(2*5)+3" ), "(2*5)+3" );
			Assert.AreEqual( "3", calc.Calculate( "sqrt(9)" ), "sqrt(9)" );
			Assert.AreEqual( "10", calc.Calculate( "log(1024,2)" ), "log(1024)" );
			Assert.AreEqual( "1", calc.Calculate( "ln(e)" ), "ln(e)" );
			Assert.AreEqual( "-10", calc.Calculate( "-2 * 5" ), "-2*5" );

			Assert.AreEqual( 1, Convert.ToDouble( calc.Calculate( "sin(90 * pi / 180)" ), CultureInfo.InvariantCulture ), 0.000001, "sin(90 * pi / 180)" );
			Assert.AreEqual( 90, Convert.ToDouble( calc.Calculate( "asin(1)*180/pi" ), CultureInfo.InvariantCulture ), 0.000001, "asin(1)*180/pi" );

			Assert.AreEqual( 0, Convert.ToDouble( calc.Calculate( "cos(90* pi / 180)" ), CultureInfo.InvariantCulture ), 0.000001, "cos(90* pi / 180)" );
			Assert.AreEqual( 90, Convert.ToDouble( calc.Calculate( "acos(0)*180/pi" ), CultureInfo.InvariantCulture ), 0.000001, "acos(0)*180/pi" );

			Assert.AreEqual( 0, Convert.ToDouble( calc.Calculate( "tan(0)" ), CultureInfo.InvariantCulture ), 0.000001, "tan(0)" );
			Assert.AreEqual( 0, Convert.ToDouble( calc.Calculate( "atan(0)" ), CultureInfo.InvariantCulture ), 0.000001, "atan(0)" );

			Assert.AreEqual( "479001600", calc.Calculate( "12!" ), "12!" );

			Assert.AreEqual( "8", calc.Calculate( "pow(2,3)" ), "pow(2,3)" );
			Assert.AreEqual( "9", calc.Calculate( "3^2" ), "3^2" );

			Assert.AreEqual( "4.51", calc.Calculate( "rnd(4.51342,2)" ), "rnd(4.51342,2)" );
			Assert.AreEqual( "5", calc.Calculate( "rnd(4.51,0)" ), "rnd(4.51,0)" );

			Assert.AreEqual( "3", calc.Calculate( "rnd(pi,0)" ), "rnd(pi,0)" );
		}
	}
}
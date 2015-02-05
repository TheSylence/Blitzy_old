using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Blitzy.Tests
{
	internal static class ExceptionAssert
	{
		public static void Throws<TException>( Action action ) where TException : Exception
		{
			try
			{
				action();
				MSAssert.Fail( "Action did not throw an exception" );
			}
			catch( TException )
			{
			}
		}
	}
}
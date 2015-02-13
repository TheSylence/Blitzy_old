using System;
using MSAssert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace Blitzy.Tests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
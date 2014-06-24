// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Utility
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class CopyFromArguments_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" )]
		public void BufferSizeTest()
		{
			CopyFromArguments args = new CopyFromArguments();
			args.BufferSize = 12345;
			Assert.AreEqual( 12345, args.BufferSize );
		}

		[TestMethod, TestCategory( "Utility" )]
		public void ConstructorTest()
		{
			CopyFromArguments args = new CopyFromArguments();
			Assert.IsNull( args.StopEvent );
			Assert.IsNull( args.ProgressChangeCallback );

			args = new CopyFromArguments( ( b, total ) => { } );
			Assert.IsNotNull( args.ProgressChangeCallback );

			args = new CopyFromArguments( ( b, total ) => { }, TimeSpan.FromSeconds( 1 ) );
			Assert.IsNotNull( args.ProgressChangeCallback );
			Assert.AreEqual( TimeSpan.FromSeconds( 1 ), args.ProgressChangeCallbackInterval );

			args = new CopyFromArguments( ( b, total ) => { }, TimeSpan.FromSeconds( 1 ), 123 );
			Assert.IsNotNull( args.ProgressChangeCallback );
			Assert.AreEqual( TimeSpan.FromSeconds( 1 ), args.ProgressChangeCallbackInterval );
			Assert.AreEqual( 123L, args.TotalLength );
		}
	}
}
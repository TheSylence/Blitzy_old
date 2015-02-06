using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	[TestClass]
	public class ConstantsTests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void DataFileTest()
		{
			Assert.IsFalse( string.IsNullOrWhiteSpace( Constants.DataFileName ) );
		}

		[TestMethod, TestCategory( "Global" )]
		public void DataPathTest()
		{
			using( ShimsContext.Create() )
			{
				bool created = false;

				System.IO.Fakes.ShimDirectory.ExistsString = ( str ) => false;
				System.IO.Fakes.ShimDirectory.CreateDirectoryString = ( str ) => { created = true; return null; };

				Assert.IsNotNull( Constants.DataPath );
				Assert.IsTrue( created );
			}

			using( ShimsContext.Create() )
			{
				bool created = false;

				System.IO.Fakes.ShimDirectory.ExistsString = ( str ) => true;
				System.IO.Fakes.ShimDirectory.CreateDirectoryString = ( str ) => { created = true; return null; };

				Assert.IsNotNull( Constants.DataPath );
				Assert.IsFalse( created );
			}
		}
	}
}
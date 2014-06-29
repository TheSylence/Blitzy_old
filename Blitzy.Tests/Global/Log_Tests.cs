// $Id$

using System;
using Blitzy.Tests.Mocks;
using log4net.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Global
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class Log_Tests : TestBase
	{
		[TestMethod, TestCategory( "Global" )]
		public void LogHelperTest()
		{
			Type type = GetType();

			using( LogChecker log = new LogChecker( Level.Fatal ) )
			{
				LogHelper.LogDebug( type, "Debug" );
				LogHelper.LogInfo( type, "Info" );
				LogHelper.LogWarning( type, "Warning" );
				LogHelper.LogError( type, "Error" );
				LogHelper.LogFatal( type, "Fatal" );

				Assert.AreEqual( 1, log.Messages.Count );
				Assert.AreEqual( "Fatal", log.Messages[0] );
			}

			using( LogChecker log = new LogChecker( Level.Error ) )
			{
				LogHelper.LogDebug( type, "Debug" );
				LogHelper.LogInfo( type, "Info" );
				LogHelper.LogWarning( type, "Warning" );
				LogHelper.LogError( type, "Error" );
				LogHelper.LogFatal( type, "Fatal" );

				Assert.AreEqual( 2, log.Messages.Count );
				Assert.AreEqual( "Error", log.Messages[0] );
				Assert.AreEqual( "Fatal", log.Messages[1] );
			}

			using( LogChecker log = new LogChecker( Level.Warn ) )
			{
				LogHelper.LogDebug( type, "Debug" );
				LogHelper.LogInfo( type, "Info" );
				LogHelper.LogWarning( type, "Warning" );
				LogHelper.LogError( type, "Error" );
				LogHelper.LogFatal( type, "Fatal" );

				Assert.AreEqual( 3, log.Messages.Count );
				Assert.AreEqual( "Warning", log.Messages[0] );
				Assert.AreEqual( "Error", log.Messages[1] );
				Assert.AreEqual( "Fatal", log.Messages[2] );
			}

			using( LogChecker log = new LogChecker( Level.Info ) )
			{
				LogHelper.LogDebug( type, "Debug" );
				LogHelper.LogInfo( type, "Info" );
				LogHelper.LogWarning( type, "Warning" );
				LogHelper.LogError( type, "Error" );
				LogHelper.LogFatal( type, "Fatal" );

				Assert.AreEqual( 4, log.Messages.Count );
				Assert.AreEqual( "Info", log.Messages[0] );
				Assert.AreEqual( "Warning", log.Messages[1] );
				Assert.AreEqual( "Error", log.Messages[2] );
				Assert.AreEqual( "Fatal", log.Messages[3] );
			}

#if DEBUG
			using( LogChecker log = new LogChecker( Level.Debug ) )
			{
				LogHelper.LogDebug( type, "Debug" );
				LogHelper.LogInfo( type, "Info" );
				LogHelper.LogWarning( type, "Warning" );
				LogHelper.LogError( type, "Error" );
				LogHelper.LogFatal( type, "Fatal" );

				Assert.AreEqual( 5, log.Messages.Count );
				Assert.AreEqual( "Debug", log.Messages[0] );
				Assert.AreEqual( "Info", log.Messages[1] );
				Assert.AreEqual( "Warning", log.Messages[2] );
				Assert.AreEqual( "Error", log.Messages[3] );
				Assert.AreEqual( "Fatal", log.Messages[4] );
			}
#endif
		}

		[TestMethod, TestCategory( "Global" )]
		public void LogObjectTest()
		{
			MockLogObject obj = new MockLogObject();

			using( LogChecker log = new LogChecker( Level.Fatal ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 1, log.Messages.Count );
				Assert.AreEqual( "Fatal", log.Messages[0] );
			}

			using( LogChecker log = new LogChecker( Level.Error ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 2, log.Messages.Count );
				Assert.AreEqual( "Error", log.Messages[0] );
				Assert.AreEqual( "Fatal", log.Messages[1] );
			}

			using( LogChecker log = new LogChecker( Level.Warn ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 3, log.Messages.Count );
				Assert.AreEqual( "Warning", log.Messages[0] );
				Assert.AreEqual( "Error", log.Messages[1] );
				Assert.AreEqual( "Fatal", log.Messages[2] );
			}

			using( LogChecker log = new LogChecker( Level.Info ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 4, log.Messages.Count );
				Assert.AreEqual( "Info", log.Messages[0] );
				Assert.AreEqual( "Warning", log.Messages[1] );
				Assert.AreEqual( "Error", log.Messages[2] );
				Assert.AreEqual( "Fatal", log.Messages[3] );
			}

#if DEBUG
			using( LogChecker log = new LogChecker( Level.Debug ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 5, log.Messages.Count );
				Assert.AreEqual( "Debug", log.Messages[0] );
				Assert.AreEqual( "Info", log.Messages[1] );
				Assert.AreEqual( "Warning", log.Messages[2] );
				Assert.AreEqual( "Error", log.Messages[3] );
				Assert.AreEqual( "Fatal", log.Messages[4] );
			}
#endif
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void ViewModelLogTests()
		{
			MockViewModel obj = new MockViewModel( "test" );
			using( LogChecker log = new LogChecker( Level.Fatal ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 1, log.Messages.Count );
				Assert.AreEqual( "Fatal", log.Messages[0] );
			}

			using( LogChecker log = new LogChecker( Level.Error ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 2, log.Messages.Count );
				Assert.AreEqual( "Error", log.Messages[0] );
				Assert.AreEqual( "Fatal", log.Messages[1] );
			}

			using( LogChecker log = new LogChecker( Level.Warn ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 3, log.Messages.Count );
				Assert.AreEqual( "Warning", log.Messages[0] );
				Assert.AreEqual( "Error", log.Messages[1] );
				Assert.AreEqual( "Fatal", log.Messages[2] );
			}

			using( LogChecker log = new LogChecker( Level.Info ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 4, log.Messages.Count );
				Assert.AreEqual( "Info", log.Messages[0] );
				Assert.AreEqual( "Warning", log.Messages[1] );
				Assert.AreEqual( "Error", log.Messages[2] );
				Assert.AreEqual( "Fatal", log.Messages[3] );
			}

#if DEBUG
			using( LogChecker log = new LogChecker( Level.Debug ) )
			{
				obj.Debug( "Debug" );
				obj.Info( "Info" );
				obj.Warning( "Warning" );
				obj.Error( "Error" );
				obj.Fatal( "Fatal" );

				Assert.AreEqual( 5, log.Messages.Count );
				Assert.AreEqual( "Debug", log.Messages[0] );
				Assert.AreEqual( "Info", log.Messages[1] );
				Assert.AreEqual( "Warning", log.Messages[2] );
				Assert.AreEqual( "Error", log.Messages[3] );
				Assert.AreEqual( "Fatal", log.Messages[4] );
			}
#endif
		}
	}
}
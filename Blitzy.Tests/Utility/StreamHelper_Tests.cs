// $Id$

using System;
using Blitzy.Tests.Mocks;
using Blitzy.Utility;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Utility
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class StreamHelper_Tests : TestBase
	{
		[TestMethod, TestCategory( "Utility" )]
		public void CopyFromTest()
		{
			ProgressStatistic stats = new ProgressStatistic();

			const int dataLength = 1000;
			byte[] data = new byte[dataLength];
			for( int i = 0; i < dataLength; ++i )
			{
				data[i] = (byte)( i % 255 );
			}

			DateTime startTime = DateTime.Now;
			bool started = false;
			bool finished = false;
			int updateCalls = 0;

			stats.Started += ( s, e ) => started = true;
			stats.Finished += ( s, e ) => finished = true;
			stats.ProgressChanged += ( s, e ) =>
				{
					updateCalls++;

					Assert.IsTrue( e.ProgressStatistic.EstimatedDuration.Ticks > 0 );
					Assert.IsTrue( e.ProgressStatistic.EstimatedFinishingTime > startTime );
					Assert.IsTrue( e.ProgressStatistic.AverageBytesPerSecond > 0 );
					Assert.IsTrue( e.ProgressStatistic.BytesRead > 0 );
					Assert.IsTrue( e.ProgressStatistic.Duration.Ticks > 0 );
					Assert.IsTrue( e.ProgressStatistic.CurrentBytesPerSecond > 0 );
					Assert.IsTrue( e.ProgressStatistic.CurrentBytesSampleCount > 0 );
					Assert.IsTrue( e.ProgressStatistic.Progress > 0 );
				};

			CopyFromArguments arguments = new CopyFromArguments( stats.ProgressChange, TimeSpan.FromMilliseconds( 10 ), data.Length );
			using( SlowStream source = new SlowStream( data, 1 ) )
			{
				using( SlowStream dest = new SlowStream( data.Length, 1 ) )
				{
					dest.CopyFrom( source, arguments );
					stats.Finish();
				}
			}

			Assert.IsTrue( started );
			Assert.IsTrue( finished );
			Assert.IsTrue( updateCalls > 0 );
		}
	}
}
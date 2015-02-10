using System;

namespace Blitzy.Utility
{
	public class ProgressEventArgs : EventArgs
	{
		public ProgressEventArgs( ProgressStatistic progressStatistic )
		{
			if( progressStatistic == null )
				throw new ArgumentNullException( "progressStatistic" );
			ProgressStatistic = progressStatistic;
		}

		public ProgressStatistic ProgressStatistic { get; private set; }
	}
}
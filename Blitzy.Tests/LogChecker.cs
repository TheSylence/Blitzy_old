// $Id$

using System;
using System.Collections.Generic;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Blitzy.Tests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class LogChecker : IDisposable
	{
		private readonly MemoryAppender Appender = new MemoryAppender();
		private readonly DebugAppender DebugAppender = new DebugAppender();
		private readonly IAppenderAttachable Logger;
		private readonly Level PreviousLevel;
		private readonly Logger Root;

		public LogChecker( Level levelToCheck )
		{
			Root = ( (log4net.Repository.Hierarchy.Hierarchy)LogManager.GetRepository() ).Root;
			Logger = Root as IAppenderAttachable;
			if( Logger != null )
			{
				Logger.AddAppender( Appender );
				Logger.AddAppender( DebugAppender );
			}

			PreviousLevel = Root.Level;
			Root.Level = levelToCheck;
		}

		public List<string> Messages
		{
			get
			{
				return new List<LoggingEvent>( Appender.GetEvents() ).ConvertAll( x => x.RenderedMessage );
			}
		}

		public void Dispose()
		{
			Root.Level = PreviousLevel;
			if( Logger != null )
			{
				Logger.RemoveAppender( Appender );
			}
		}
	}
}
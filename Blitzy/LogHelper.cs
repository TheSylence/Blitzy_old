// $Id$

using System;
using System.Globalization;
using log4net;

namespace Blitzy
{
	internal static class LogHelper
	{
		#region Methods

		public static void LogDebug( Type type, string format, params object[] args )
		{
#if DEBUG
			ILog log = LogManager.GetLogger( type );

			if( log.IsDebugEnabled )
			{
				log.DebugFormat( CultureInfo.InvariantCulture, format, args );
			}
#endif
		}

		public static void LogError( Type type, string format, params object[] args )
		{
			ILog log = LogManager.GetLogger( type );

			if( log.IsErrorEnabled )
			{
				log.ErrorFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		public static void LogFatal( Type type, string format, params object[] args )
		{
			ILog log = LogManager.GetLogger( type );

			if( log.IsFatalEnabled )
			{
				log.FatalFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		public static void LogInfo( Type type, string format, params object[] args )
		{
			ILog log = LogManager.GetLogger( type );

			if( log.IsInfoEnabled )
			{
				log.InfoFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		public static void LogWarning( Type type, string format, params object[] args )
		{
			ILog log = LogManager.GetLogger( type );

			if( log.IsWarnEnabled )
			{
				log.WarnFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		#endregion Methods
	}
}
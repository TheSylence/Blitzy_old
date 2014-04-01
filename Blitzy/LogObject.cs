// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace Blitzy
{
	public class LogObject
	{
		#region Constructor

		public LogObject()
		{
			Log = LogManager.GetLogger( GetType() );
		}

		#endregion Constructor

		#region Methods

		protected ILog Log;

		protected void LogDebug( string format, params object[] args )
		{
			if( Log.IsDebugEnabled )
			{
				Log.DebugFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogError( string format, params object[] args )
		{
			if( Log.IsErrorEnabled )
			{
				Log.ErrorFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogFatal( string format, params object[] args )
		{
			if( Log.IsFatalEnabled )
			{
				Log.FatalFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogInfo( string format, params object[] args )
		{
			if( Log.IsInfoEnabled )
			{
				Log.InfoFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		protected void LogWarning( string format, params object[] args )
		{
			if( Log.IsWarnEnabled )
			{
				Log.WarnFormat( CultureInfo.InvariantCulture, format, args );
			}
		}

		#endregion Methods
	}
}
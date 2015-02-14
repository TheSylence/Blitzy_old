using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy
{
	public static class AsyncErrorHandler
	{
		public static void HandleException( Exception exception )
		{
			LogHelper.LogFatal( typeof( AsyncErrorHandler ), "Unhandled exception in async task: {0}", exception );
		}
	}
}
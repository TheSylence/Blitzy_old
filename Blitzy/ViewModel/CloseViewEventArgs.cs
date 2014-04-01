using System;

namespace Blitzy.ViewModel
{
	public class CloseViewEventArgs : EventArgs
	{
		public readonly bool? Result;

		private static CloseViewEventArgs _Default;

		public CloseViewEventArgs( bool? result = null )
		{
			Result = result;
		}

		public static CloseViewEventArgs Default
		{
			get
			{
				if( _Default == null )
					_Default = new CloseViewEventArgs();

				return _Default;
			}
		}
	}
}
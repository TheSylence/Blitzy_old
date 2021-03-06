﻿using System;

namespace Blitzy.ViewModel
{
	public class CloseViewEventArgs : EventArgs
	{
		public CloseViewEventArgs( bool? result = null )
		{
			Result = result;
		}

		public static CloseViewEventArgs Default
		{
			get { return _Default ?? ( _Default = new CloseViewEventArgs() ); }
		}

		public readonly bool? Result;
		private static CloseViewEventArgs _Default;
	}
}
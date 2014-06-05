// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace Blitzy.Messages
{
	internal class BalloonTipMessage : MessageBase
	{
		public readonly BalloonIcon Icon;
		public readonly string Message;
		public readonly string Title;
		public readonly object Token;

		public BalloonTipMessage( string title, string message, BalloonIcon icon, object token = null )
		{
			Message = message;
			Icon = icon;
			Title = title;
			Token = token;
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	internal class BalloonActivatedMessage : MessageBase
	{
		public readonly object Token;

		public BalloonActivatedMessage( object token )
		{
			Token = token;
		}
	}
}
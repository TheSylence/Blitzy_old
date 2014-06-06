// $Id$

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
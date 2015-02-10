using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class BalloonActivatedMessage : MessageBase
	{
		public BalloonActivatedMessage( object token )
		{
			Token = token;
		}

		public readonly object Token;
	}
}
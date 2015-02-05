

using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class BalloonActivatedMessage : MessageBase
	{
		public readonly object Token;

		public BalloonActivatedMessage( object token )
		{
			Token = token;
		}
	}
}
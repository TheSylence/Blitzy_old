using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InputCaretPositionMessage : MessageBase
	{
		public InputCaretPositionMessage( int index )
		{
			Index = index;
		}

		public readonly int Index;
	}
}
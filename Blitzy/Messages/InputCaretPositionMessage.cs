// $Id$

using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InputCaretPositionMessage : MessageBase
	{
		public readonly int Index;

		public InputCaretPositionMessage( int index )
		{
			Index = index;
		}
	}
}
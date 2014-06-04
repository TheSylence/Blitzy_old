// $Id$

using System.Diagnostics.CodeAnalysis;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InputCaretPositionMessage
	{
		public readonly int Index;

		public InputCaretPositionMessage( int index )
		{
			Index = index;
		}
	}
}
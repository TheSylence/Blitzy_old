// $Id$

using System.Diagnostics.CodeAnalysis;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InternalCommandMessage
	{
		public readonly string Command;

		public InternalCommandMessage( string command )
		{
			Command = command;
		}
	}
}


using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InternalCommandMessage : MessageBase
	{
		public readonly string Command;

		public InternalCommandMessage( string command )
		{
			Command = command;
		}
	}
}
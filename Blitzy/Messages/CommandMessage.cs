

using System.Diagnostics.CodeAnalysis;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	internal enum CommandStatus
	{
		Executing,
		Finished,
		Error
	}

	[ExcludeFromCodeCoverage]
	internal class CommandMessage : MessageBase
	{
		public readonly string Message;
		public readonly CommandStatus Status;
		public readonly int? TaskID;

		public CommandMessage( CommandStatus status, string message, int? taskId )
		{
			Status = status;
			Message = message;
			TaskID = taskId;
		}
	}
}
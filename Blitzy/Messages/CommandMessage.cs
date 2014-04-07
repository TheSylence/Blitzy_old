// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Messages
{
	internal enum CommandStatus
	{
		Executing,
		Finished,
		Error
	}

	[ExcludeFromCodeCoverage]
	internal class CommandMessage
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
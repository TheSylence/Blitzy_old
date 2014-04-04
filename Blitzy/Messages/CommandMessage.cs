// $Id$

using System;
using System.Collections.Generic;
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

	internal class CommandMessage
	{
		public readonly string Message;
		public readonly CommandStatus Status;

		public CommandMessage( CommandStatus status, string message = null )
		{
			Status = status;
			Message = message;
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;

namespace Blitzy.Messages
{
	internal enum HistoryMessageType
	{
		Show,
		Up,
		Down,
		Hide
	}

	internal class HistoryMessage
	{
		public readonly HistoryManager History;
		public readonly HistoryMessageType Type;

		public HistoryMessage( HistoryMessageType type, HistoryManager history = null )
		{
			Type = type;
			History = history;
		}
	}
}
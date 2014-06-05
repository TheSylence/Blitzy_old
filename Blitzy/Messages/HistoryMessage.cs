// $Id$

using System.Diagnostics.CodeAnalysis;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	internal enum HistoryMessageType
	{
		Show,
		Up,
		Down,
		Hide
	}

	[ExcludeFromCodeCoverage]
	internal class HistoryMessage : MessageBase
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
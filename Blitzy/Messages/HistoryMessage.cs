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
		public HistoryMessage( HistoryMessageType type, HistoryManager history = null )
		{
			Type = type;
			History = history;
		}

		public readonly HistoryManager History;
		public readonly HistoryMessageType Type;
	}
}
using System;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Plugin
{
	public interface IPluginHost
	{
		bool IsPluginLoaded( Guid id );

		DbConnectionFactory ConnectionFactory { get; }

		IMessenger Messenger { get; }

		ISettings Settings { get; }
	}
}
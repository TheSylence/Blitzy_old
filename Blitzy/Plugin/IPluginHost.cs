using System;

namespace Blitzy.Plugin
{
	public interface IPluginHost
	{
		bool IsPluginLoaded( Guid id );

		DbConnectionFactory ConnectionFactory { get; }

		ISettings Settings { get; }
	}
}
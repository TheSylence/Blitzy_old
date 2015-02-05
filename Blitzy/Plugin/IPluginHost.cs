// $Id$

using System;

namespace Blitzy.Plugin
{
	public interface IPluginHost
	{
		bool IsPluginLoaded( Guid id );

		IDatabase Database { get; }

		ISettings Settings { get; }
	}
}
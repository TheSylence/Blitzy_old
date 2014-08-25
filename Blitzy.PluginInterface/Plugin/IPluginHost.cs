// $Id$

using System;

namespace Blitzy.Plugin
{
	public interface IPluginHost
	{
		#region Methods

		bool IsPluginLoaded( Guid id );

		#endregion Methods

		#region Properties

		IDatabase Database { get; }

		ISettings Settings { get; }

		#endregion Properties
	}
}
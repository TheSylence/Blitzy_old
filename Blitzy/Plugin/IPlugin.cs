// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin
{
	public enum PluginUnloadReason
	{
		Shutdown,
		Unload
	}

	public interface IPlugin
	{
		#region Methods

		void ClearCache();

		bool Load( IPluginHost host, string oldVersion = null );

		void Unload( PluginUnloadReason reason );

		#endregion Methods

		#region Properties

		Guid PluginID { get; }

		#endregion Properties
	}
}
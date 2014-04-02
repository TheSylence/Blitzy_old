// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin
{
	public interface IPluginHost
	{
		#region Methods

		bool IsPluginLoaded( Guid id );

		#endregion Methods

		#region Properties

		ISettings Settings { get; }

		#endregion Properties
	}
}
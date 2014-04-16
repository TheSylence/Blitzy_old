// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.ViewServices;

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
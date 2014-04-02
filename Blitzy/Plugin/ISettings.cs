// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin
{
	public interface ISettings
	{
		#region Methods

		T GetValue<T>( IPlugin plugin, string key );

		void RemoveValue( IPlugin plugin, string key );

		void SetValue( IPlugin plugin, string key, object value );

		#endregion Methods
	}
}
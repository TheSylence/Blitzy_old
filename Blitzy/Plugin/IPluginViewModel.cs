using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin
{
	public interface IPluginViewModel
	{
		void RestoreDefaults();

		void Save();
	}
}
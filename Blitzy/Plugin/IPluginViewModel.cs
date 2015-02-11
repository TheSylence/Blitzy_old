using System;

namespace Blitzy.Plugin
{
	public interface IPluginViewModel : IDisposable
	{
		void RestoreDefaults();

		void Save();
	}
}
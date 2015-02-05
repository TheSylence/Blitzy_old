using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.ViewServices;

namespace Blitzy.ViewModel
{
	internal abstract class SettingsViewModelBase : ViewModelBaseEx
	{
		protected SettingsViewModelBase( Settings settings, IViewServiceManager serviceManager = null )
			: base( serviceManager as ViewServiceManager )
		{
			Settings = settings;
		}

		public abstract void Save();

		protected Settings Settings { get; private set; }
	}
}
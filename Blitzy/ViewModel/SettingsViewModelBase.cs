using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.ViewServices;

namespace Blitzy.ViewModel
{
	internal abstract class SettingsViewModelBase : ViewModelBaseEx
	{
		protected SettingsViewModelBase( Settings settings, DbConnectionFactory connectionFactory, IViewServiceManager serviceManager = null )
			: base( connectionFactory, serviceManager as ViewServiceManager )
		{
			Settings = settings;
		}

		public abstract void Save();

		protected Settings Settings { get; private set; }
	}
}
using Blitzy.Model;

namespace Blitzy.ViewModel
{
	internal abstract class SettingsViewModelBase : ViewModelBaseEx
	{
		#region Constructor

		protected SettingsViewModelBase( Settings settings )
		{
			Settings = settings;
		}

		#endregion Constructor

		#region Methods

		public abstract void Save();

		#endregion Methods

		#region Properties

		protected Settings Settings { get; private set; }

		#endregion Properties
	}
}
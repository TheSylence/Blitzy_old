using Blitzy.Model;

namespace Blitzy.ViewModel
{
	internal abstract class SettingsViewModelBase : ViewModelBaseEx
	{
		#region Constructor

		protected SettingsViewModelBase( SettingsViewModel baseVm )
		{
			BaseVm = baseVm;
		}

		#endregion Constructor

		#region Methods

		public abstract void Save();

		#endregion Methods

		#region Properties

		protected Settings Settings
		{
			get
			{
				return BaseVm.Settings;
			}
		}

		#endregion Properties

		#region Attributes

		protected readonly SettingsViewModel BaseVm;

		#endregion Attributes
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blitzy.ViewModel
{
	internal abstract class SettingsViewModelBase : ViewModelBaseEx
	{
		#region Constructor

		public SettingsViewModelBase( SettingsViewModel baseVM )
		{
			BaseVM = baseVM;
		}

		#endregion Constructor

		#region Methods

		public abstract void Save();

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		protected readonly SettingsViewModel BaseVM;

		#endregion Attributes
	}
}
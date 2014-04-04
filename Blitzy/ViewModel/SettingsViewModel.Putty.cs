// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class PuttySettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public PuttySettingsViewModel( SettingsViewModel baseVM )
			: base( baseVM )
		{
		}

		#endregion Constructor

		#region Methods

		public override void Save()
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Commands

		private RelayCommand _BrowsePuttyCommand;
		private RelayCommand _ImportSessionsCommand;

		public RelayCommand BrowsePuttyCommand
		{
			get
			{
				return _BrowsePuttyCommand ??
					( _BrowsePuttyCommand = new RelayCommand( ExecuteBrowsePuttyCommand, CanExecuteBrowsePuttyCommand ) );
			}
		}

		public RelayCommand ImportSessionsCommand
		{
			get
			{
				return _ImportSessionsCommand ??
					( _ImportSessionsCommand = new RelayCommand( ExecuteImportSessionsCommand, CanExecuteImportSessionsCommand ) );
			}
		}

		private bool CanExecuteBrowsePuttyCommand()
		{
			return true;
		}

		private bool CanExecuteImportSessionsCommand()
		{
			return true;
		}

		private void ExecuteBrowsePuttyCommand()
		{
		}

		private void ExecuteImportSessionsCommand()
		{
		}

		#endregion Commands

		#region Properties

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
﻿// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.ViewModel
{
	internal class WinySettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public WinySettingsViewModel( SettingsViewModel baseVM )
			: base( baseVM )
		{
		}

		#endregion Constructor

		public override void Save()
		{
			throw new NotImplementedException();
		}

		#region Methods

		#endregion Methods

		#region Properties

		private bool _LogoffConfirmation;
		private bool _RestartConfirmation;
		private bool _ShutdownConfirmation;

		public bool LogoffConfirmation
		{
			get
			{
				return _LogoffConfirmation;
			}

			set
			{
				if( _LogoffConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => LogoffConfirmation );
				_LogoffConfirmation = value;
				RaisePropertyChanged( () => LogoffConfirmation );
			}
		}

		public bool RestartConfirmation
		{
			get
			{
				return _RestartConfirmation;
			}

			set
			{
				if( _RestartConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => RestartConfirmation );
				_RestartConfirmation = value;
				RaisePropertyChanged( () => RestartConfirmation );
			}
		}

		public bool ShutdownConfirmation
		{
			get
			{
				return _ShutdownConfirmation;
			}

			set
			{
				if( _ShutdownConfirmation == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShutdownConfirmation );
				_ShutdownConfirmation = value;
				RaisePropertyChanged( () => ShutdownConfirmation );
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
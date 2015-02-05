// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;
using GalaSoft.MvvmLight.Command;
using LinqToTwitter;

namespace Blitzy140
{
	internal class ViewModel : IPluginViewModel
	{
		#region Methods

		public void RestoreDefaults()
		{
			throw new NotImplementedException();
		}

		public void Save()
		{
			throw new NotImplementedException();
		}

		private PinAuthorizer GetAuthorizer()
		{
			return new PinAuthorizer()
			{
				CredentialStore = new InMemoryCredentialStore()
				{
					// TODO: Provide Keys
				},
				SupportsCompression = true,
				GoToTwitterAuthorization = link => Process.Start( link ),
				GetPin = () =>
				{
					// TODO: Read pin from user input
					return null;
				}
			};
		}

		#endregion Methods

		#region Commands

		private RelayCommand _AddAccountCommand;

		private RelayCommand _DeleteAccountCommand;

		public RelayCommand AddAccountCommand
		{
			get
			{
				return _AddAccountCommand ??
					( _AddAccountCommand = new RelayCommand( ExecuteAddAccountCommand, CanExecuteAddAccountCommand ) );
			}
		}

		public RelayCommand DeleteAccountCommand
		{
			get
			{
				return _DeleteAccountCommand ??
					( _DeleteAccountCommand = new RelayCommand( ExecuteDeleteAccountCommand, CanExecuteDeleteAccountCommand ) );
			}
		}

		private bool CanExecuteAddAccountCommand()
		{
			return true;
		}

		private bool CanExecuteDeleteAccountCommand()
		{
			return true;
		}

		private void ExecuteAddAccountCommand()
		{
			PinAuthorizer authorizer = GetAuthorizer();

			authorizer.AuthorizeAsync();
		}

		private void ExecuteDeleteAccountCommand()
		{
		}

		#endregion Commands
	}
}
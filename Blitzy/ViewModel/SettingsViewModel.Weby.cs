// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Model;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class WebySettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public WebySettingsViewModel( SettingsViewModel baseVM )
			: base( baseVM )
		{
			Websites = new ObservableCollection<WebyWebsite>();

			using( SQLiteCommand cmd = BaseVM.Settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WebyID FROM weby";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						WebyWebsite site = new WebyWebsite();
						site.ID = reader.GetInt32( 0 );

						site.Load( BaseVM.Settings.Connection );
						Websites.Add( site );
					}
				}
			}
		}

		#endregion Constructor

		#region Methods

		public override void Save()
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Commands

		private RelayCommand _AddWebsiteCommand;
		private RelayCommand _RemoveWebsiteCommand;

		public RelayCommand AddWebsiteCommand
		{
			get
			{
				return _AddWebsiteCommand ??
					( _AddWebsiteCommand = new RelayCommand( ExecuteAddWebsiteCommand, CanExecuteAddWebsiteCommand ) );
			}
		}

		public RelayCommand RemoveWebsiteCommand
		{
			get
			{
				return _RemoveWebsiteCommand ??
					( _RemoveWebsiteCommand = new RelayCommand( ExecuteRemoveWebsiteCommand, CanExecuteRemoveWebsiteCommand ) );
			}
		}

		private bool CanExecuteAddWebsiteCommand()
		{
			return true;
		}

		private bool CanExecuteRemoveWebsiteCommand()
		{
			return SelectedWebsite != null;
		}

		private void ExecuteAddWebsiteCommand()
		{
			WebyWebsite site = DialogServiceManager.Create<WebyWebsite>();
			if( site != null )
			{
				Websites.Add( site );
			}
		}

		private void ExecuteRemoveWebsiteCommand()
		{
			string text = "ConfirmDeleteItem".Localize();
			string caption = "Question".Localize();
			MessageBoxParameter args = new MessageBoxParameter( text, caption );

			MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( args );
			if( result == MessageBoxResult.Yes )
			{
				Websites.Remove( SelectedWebsite );
				SelectedWebsite = null;
			}
		}

		#endregion Commands

		#region Properties

		private WebyWebsite _SelectedWebsite;

		public WebyWebsite SelectedWebsite
		{
			get
			{
				return _SelectedWebsite;
			}

			set
			{
				if( _SelectedWebsite == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedWebsite );
				_SelectedWebsite = value;
				RaisePropertyChanged( () => SelectedWebsite );
			}
		}

		public ObservableCollection<WebyWebsite> Websites { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
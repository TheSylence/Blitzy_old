// $Id$

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Windows;
using Blitzy.Model;
using Blitzy.Plugin;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	internal class WebySettingsViewModel : SettingsViewModelBase, IPluginViewModel
	{
		#region Constructor

		public WebySettingsViewModel( Settings settings )
			: base( settings )
		{
			Websites = new ObservableCollection<WebyWebsite>();

			using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WebyID FROM weby_websites";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						WebyWebsite site = new WebyWebsite { ID = reader.GetInt32( 0 ) };

						site.Load( Settings.Connection );
						Websites.Add( site );
					}
				}
			}
		}

		#endregion Constructor

		#region Methods

		public void RestoreDefaults()
		{
			throw new System.NotImplementedException();
		}

		public override void Save()
		{
			foreach( WebyWebsite site in WebsitesToRemove )
			{
				site.Delete( Settings.Connection );
			}

			foreach( WebyWebsite site in Websites )
			{
				site.Save( Settings.Connection );
			}
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
				WebsitesToRemove.Add( SelectedWebsite );
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

		private readonly List<WebyWebsite> WebsitesToRemove = new List<WebyWebsite>();

		#endregion Attributes
	}
}
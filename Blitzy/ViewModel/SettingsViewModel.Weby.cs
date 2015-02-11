using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Common;
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
		public WebySettingsViewModel( DbConnectionFactory factory, Settings settings, IViewServiceManager serviceManager = null )
			: base( settings, factory, serviceManager )
		{
			Websites = new ObservableCollection<WebyWebsite>();

			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT WebyID FROM weby_websites";

					using( DbDataReader reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							WebyWebsite site = ToDispose( new WebyWebsite { ID = reader.GetInt32( 0 ) } );

							site.Load( connection );
							Websites.Add( site );
						}
					}
				}
			}
		}

		public void RestoreDefaults()
		{
			throw new System.NotImplementedException();
		}

		public override void Save()
		{
			using( DbConnection connection = ConnectionFactory.OpenConnection() )
			{
				foreach( WebyWebsite site in WebsitesToRemove )
				{
					site.Delete( connection );
				}

				foreach( WebyWebsite site in Websites )
				{
					site.Save( connection );
				}
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
			WebyWebsite site = ServiceManagerInstance.Create<WebyWebsite>();
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

			MessageBoxResult result = ServiceManagerInstance.Show<MessageBoxService, MessageBoxResult>( args );
			if( result == MessageBoxResult.Yes )
			{
				WebsitesToRemove.Add( SelectedWebsite );
				Websites.Remove( SelectedWebsite );
				SelectedWebsite = null;
			}
		}

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

		private readonly List<WebyWebsite> WebsitesToRemove = new List<WebyWebsite>();
		private RelayCommand _AddWebsiteCommand;
		private RelayCommand _RemoveWebsiteCommand;
		private WebyWebsite _SelectedWebsite;
	}
}
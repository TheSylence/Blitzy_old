using System;
using System.Linq;
using System.Text;
using Blitzy.btbapi;
using Blitzy.Model;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.CommandWpf;

namespace Blitzy.ViewModel.Dialogs
{
	public class ChangelogDialogViewModel : ViewModelBaseEx
	{
		public ChangelogDialogViewModel( ViewServiceManager serviceManager = null )
			: base( null, serviceManager )
		{
		}

		private bool CanExecuteDownloadCommand()
		{
			return LatestVersionInfo != null;
		}

		private void ExecuteDownloadCommand()
		{
			Close();
			UpdateChecker.Instance.DownloadLatestVersion( LatestVersionInfo, ServiceManagerInstance );
		}

		public string Changelog
		{
			get
			{
				return _Changelog;
			}

			set
			{
				if( _Changelog == value )
				{
					return;
				}

				_Changelog = value;
				RaisePropertyChanged( () => Changelog );
			}
		}

		public RelayCommand DownloadCommand
		{
			get
			{
				return _DownloadCommand ??
					( _DownloadCommand = new RelayCommand( ExecuteDownloadCommand, CanExecuteDownloadCommand ) );
			}
		}

		public VersionInfo LatestVersionInfo
		{
			get
			{
				return _LatestVersionInfo;
			}

			set
			{
				if( _LatestVersionInfo == value )
				{
					return;
				}

				_LatestVersionInfo = value;
				RaisePropertyChanged( () => LatestVersionInfo );

				if( LatestVersionInfo != null )
				{
					StringBuilder sb = new StringBuilder();

					if( LatestVersionInfo.ChangeLogs != null )
					{
						foreach( Version v in LatestVersionInfo.ChangeLogs.Keys.OrderByDescending( k => k ) )
						{
							sb.AppendFormat( "<u>Changes in Version {0}:</u><br />", v );
							sb.AppendLine( LatestVersionInfo.ChangeLogs[v] );
							sb.AppendLine();
							sb.AppendLine();
						}
					}
					else
					{
						sb.AppendLine( "No changelog available" );
					}

					Changelog = sb.ToString();
				}
			}
		}

		private string _Changelog;
		private RelayCommand _DownloadCommand;
		private VersionInfo _LatestVersionInfo;
	}
}
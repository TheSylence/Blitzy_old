// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using btbapi;

namespace Blitzy.ViewModel.Dialogs
{
	public class ChangelogDialogViewModel : ViewModelBaseEx
	{
		#region Properties

		private string _Changelog;
		private VersionInfo _LatestVersionInfo;

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

				RaisePropertyChanging( () => Changelog );
				_Changelog = value;
				RaisePropertyChanged( () => Changelog );
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

				RaisePropertyChanging( () => LatestVersionInfo );
				_LatestVersionInfo = value;
				RaisePropertyChanged( () => LatestVersionInfo );

				if( LatestVersionInfo != null )
				{
					StringBuilder sb = new StringBuilder();

					foreach( Version v in LatestVersionInfo.ChangeLogs.Keys.OrderByDescending( k => k ) )
					{
						sb.AppendFormat( "<u>Changes in Version {0}:</u><br />", v );
						sb.AppendLine( LatestVersionInfo.ChangeLogs[v] );
						sb.AppendLine();
						sb.AppendLine();
					}

					Changelog = sb.ToString();
				}
			}
		}

		#endregion Properties
	}
}
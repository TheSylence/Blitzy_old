// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.ViewModel
{
	internal class DownloadDialogViewModel : ViewModelBaseEx
	{
		#region Constructor

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Properties

		private string _DownloadLink;
		private ulong _FileSize;
		private string _MD5;
		private string _TargetPath;

		public string DownloadLink
		{
			get
			{
				return _DownloadLink;
			}

			set
			{
				if( _DownloadLink == value )
				{
					return;
				}

				RaisePropertyChanging( () => DownloadLink );
				_DownloadLink = value;
				RaisePropertyChanged( () => DownloadLink );
			}
		}

		public ulong FileSize
		{
			get
			{
				return _FileSize;
			}

			set
			{
				if( _FileSize == value )
				{
					return;
				}

				RaisePropertyChanging( () => FileSize );
				_FileSize = value;
				RaisePropertyChanged( () => FileSize );
			}
		}

		public string MD5
		{
			get
			{
				return _MD5;
			}

			set
			{
				if( _MD5 == value )
				{
					return;
				}

				RaisePropertyChanging( () => MD5 );
				_MD5 = value;
				RaisePropertyChanged( () => MD5 );
			}
		}

		public string TargetPath
		{
			get
			{
				return _TargetPath;
			}

			set
			{
				if( _TargetPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => TargetPath );
				_TargetPath = value;
				RaisePropertyChanged( () => TargetPath );
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
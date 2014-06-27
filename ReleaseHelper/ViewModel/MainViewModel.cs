using System;
using System.Threading.Tasks;
using System.Windows.Input;
using btbapi;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace ReleaseHelper.ViewModel
{
	public class MainViewModel : ViewModelBase
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			DispatcherHelper.Initialize();
#if DEBUG
			Api = new API( APIEndPoint.Localhost );
#else
			Api = new API( APIEndPoint.Default );
#endif
			Task.Run( async () =>
				{
					VersionInfo info = await Api.CheckVersion( "Blitzy" );
					CurrentVersion = info.LatestVersion;
					DispatcherHelper.CheckBeginInvokeOnUI( () => CommandManager.InvalidateRequerySuggested() );
				} );
			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}

			ShouldCreateTag = true;
			ShouldPublish = true;
			ShouldUpload = true;
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Commands

		private RelayCommand _BuildVersionCommand;
		private RelayCommand _MajorVersionCommand;
		private RelayCommand _MinorVersionCommand;
		private RelayCommand _ReleaseCommand;

		public RelayCommand BuildVersionCommand
		{
			get
			{
				return _BuildVersionCommand ??
					( _BuildVersionCommand = new RelayCommand( ExecuteBuildVersionCommand, CanExecuteBuildVersionCommand ) );
			}
		}

		public RelayCommand MajorVersionCommand
		{
			get
			{
				return _MajorVersionCommand ??
					( _MajorVersionCommand = new RelayCommand( ExecuteMajorVersionCommand, CanExecuteMajorVersionCommand ) );
			}
		}

		public RelayCommand MinorVersionCommand
		{
			get
			{
				return _MinorVersionCommand ??
					( _MinorVersionCommand = new RelayCommand( ExecuteMinorVersionCommand, CanExecuteMinorVersionCommand ) );
			}
		}

		public RelayCommand ReleaseCommand
		{
			get
			{
				return _ReleaseCommand ??
					( _ReleaseCommand = new RelayCommand( ExecuteReleaseCommand, CanExecuteReleaseCommand ) );
			}
		}

		private bool CanExecuteBuildVersionCommand()
		{
			return CurrentVersion != null;
		}

		private bool CanExecuteMajorVersionCommand()
		{
			return CurrentVersion != null;
		}

		private bool CanExecuteMinorVersionCommand()
		{
			return CurrentVersion != null;
		}

		private bool CanExecuteReleaseCommand()
		{
			return VersionOk && TargetVersion != null;
		}

		private void ExecuteBuildVersionCommand()
		{
			TargetVersion = new Version( CurrentVersion.Major, CurrentVersion.Minor, CurrentVersion.Build + 1 );
		}

		private void ExecuteMajorVersionCommand()
		{
			TargetVersion = new Version( CurrentVersion.Major + 1, 0, 0 );
		}

		private void ExecuteMinorVersionCommand()
		{
			TargetVersion = new Version( CurrentVersion.Major, CurrentVersion.Minor + 1, 0 );
		}

		private void ExecuteReleaseCommand()
		{
		}

		#endregion Commands

		#region Properties

		private string _Changelog;
		private Version _CurrentVersion;
		private bool _ShouldBlogPost;
		private bool _ShouldCreateTag;
		private bool _ShouldPublish;
		private bool _ShouldUpload;
		private Version _TargetVersion;
		private bool _VersionOk;

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

		public Version CurrentVersion
		{
			get
			{
				return _CurrentVersion;
			}

			set
			{
				if( _CurrentVersion == value )
				{
					return;
				}

				RaisePropertyChanging( () => CurrentVersion );
				_CurrentVersion = value;
				RaisePropertyChanged( () => CurrentVersion );
			}
		}

		public bool ShouldBlogPost
		{
			get
			{
				return _ShouldBlogPost;
			}

			set
			{
				if( _ShouldBlogPost == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShouldBlogPost );
				_ShouldBlogPost = value;
				RaisePropertyChanged( () => ShouldBlogPost );
			}
		}

		public bool ShouldCreateTag
		{
			get
			{
				return _ShouldCreateTag;
			}

			set
			{
				if( _ShouldCreateTag == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShouldCreateTag );
				_ShouldCreateTag = value;
				RaisePropertyChanged( () => ShouldCreateTag );
			}
		}

		public bool ShouldPublish
		{
			get
			{
				return _ShouldPublish;
			}

			set
			{
				if( _ShouldPublish == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShouldPublish );
				_ShouldPublish = value;
				RaisePropertyChanged( () => ShouldPublish );
			}
		}

		public bool ShouldUpload
		{
			get
			{
				return _ShouldUpload;
			}

			set
			{
				if( _ShouldUpload == value )
				{
					return;
				}

				RaisePropertyChanging( () => ShouldUpload );
				_ShouldUpload = value;
				RaisePropertyChanged( () => ShouldUpload );
			}
		}

		public Version TargetVersion
		{
			get
			{
				return _TargetVersion;
			}

			set
			{
				if( _TargetVersion == value )
				{
					return;
				}

				RaisePropertyChanging( () => TargetVersion );
				_TargetVersion = value;
				RaisePropertyChanged( () => TargetVersion );
			}
		}

		public bool VersionOk
		{
			get
			{
				return _VersionOk;
			}

			set
			{
				if( _VersionOk == value )
				{
					return;
				}

				RaisePropertyChanging( () => VersionOk );
				_VersionOk = value;
				RaisePropertyChanged( () => VersionOk );
			}
		}

		#endregion Properties

		#region Attributes

		private API Api;

		#endregion Attributes
	}
}
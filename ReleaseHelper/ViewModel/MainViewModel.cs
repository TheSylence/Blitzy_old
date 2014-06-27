using System;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;
using btbapi;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Threading;

namespace ReleaseHelper.ViewModel
{
	public class BuildInformation : ObservableObject
	{
		#region Constructor

		public BuildInformation()
		{
			string currentDir = Directory.GetCurrentDirectory();
			SolutionDir = Directory.GetParent( currentDir ).Parent.Parent.FullName;
			OutputPath = Path.Combine( SolutionDir, "Output" );
			WixFile = Path.Combine( SolutionDir, "Setup", Constants.Files.WixProjectFile );
			BuildFile = Path.Combine( SolutionDir, "release.msbuild" );

			SetupFile = Path.Combine( OutputPath, "Setup", "Blitzy.msi" );
		}

		#endregion Constructor

		#region Properties

		private string _BuildFile;
		private string _OutputPath;
		private string _SetupFile;
		private string _SolutionDir;
		private string _WixFile;

		public string BuildFile
		{
			get
			{
				return _BuildFile;
			}

			set
			{
				if( _BuildFile == value )
				{
					return;
				}

				RaisePropertyChanging( () => BuildFile );
				_BuildFile = value;
				RaisePropertyChanged( () => BuildFile );
			}
		}

		public string OutputPath
		{
			get
			{
				return _OutputPath;
			}

			set
			{
				if( _OutputPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => OutputPath );
				_OutputPath = value;
				RaisePropertyChanged( () => OutputPath );
			}
		}

		public string SetupFile
		{
			get
			{
				return _SetupFile;
			}

			set
			{
				if( _SetupFile == value )
				{
					return;
				}

				RaisePropertyChanging( () => SetupFile );
				_SetupFile = value;
				RaisePropertyChanged( () => SetupFile );
			}
		}

		public string SolutionDir
		{
			get
			{
				return _SolutionDir;
			}

			set
			{
				if( _SolutionDir == value )
				{
					return;
				}

				RaisePropertyChanging( () => SolutionDir );
				_SolutionDir = value;
				RaisePropertyChanged( () => SolutionDir );
			}
		}

		public string WixFile
		{
			get
			{
				return _WixFile;
			}

			set
			{
				if( _WixFile == value )
				{
					return;
				}

				RaisePropertyChanging( () => WixFile );
				_WixFile = value;
				RaisePropertyChanged( () => WixFile );
			}
		}

		#endregion Properties
	}

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

			TargetVersion = new Version( File.ReadAllText( "../../../Blitzy/Properties/Version.txt" ).Trim() );

			MaximumSteps = int.MaxValue;
			ShouldCreateTag = true;
			ShouldPublish = true;
			ShouldUpload = true;
			BuildInfo = new BuildInformation();
		}

		#endregion Constructor

		#region Methods

		private void CreateTag()
		{
		}

		private void PublishVersion()
		{
		}

		private void RunMSBuild()
		{
		}

		private void UploadToFtp()
		{
		}

		private void WriteBlogPost()
		{
		}

		#endregion Methods

		#region Commands

		private RelayCommand _ReleaseCommand;

		public RelayCommand ReleaseCommand
		{
			get
			{
				return _ReleaseCommand ??
					( _ReleaseCommand = new RelayCommand( ExecuteReleaseCommand, CanExecuteReleaseCommand ) );
			}
		}

		private bool CanExecuteReleaseCommand()
		{
			return VersionOk && TargetVersion != null;
		}

		private void ExecuteReleaseCommand()
		{
			Worker = new BackgroundWorker();
			Worker.DoWork += Worker_DoWork;
			Worker.ProgressChanged += Worker_ProgressChanged;
			Worker.RunWorkerCompleted += Worker_RunWorkerCompleted;

			Worker.RunWorkerAsync();
		}

		private void Worker_DoWork( object sender, DoWorkEventArgs e )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
			{
				MaximumSteps = 5;
			} );

			int step = 0;
			Worker.ReportProgress( step++, "Building solution..." );
			RunMSBuild();

			Worker.ReportProgress( step++, "Creating Tag..." );
			if( ShouldCreateTag )
			{
				CreateTag();
			}

			Worker.ReportProgress( step++, "Uploading to FTP..." );
			if( ShouldUpload )
			{
				UploadToFtp();
			}

			Worker.ReportProgress( step++, "Publishing Version..." );
			if( ShouldPublish )
			{
				PublishVersion();
			}

			Worker.ReportProgress( step++, "Writing Blog post..." );
			if( ShouldBlogPost )
			{
				WriteBlogPost();
			}
		}

		private void Worker_ProgressChanged( object sender, ProgressChangedEventArgs e )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
				{
					CurrentStep = e.ProgressPercentage;
					ProgressText = (string)e.UserState;
				} );
		}

		private void Worker_RunWorkerCompleted( object sender, RunWorkerCompletedEventArgs e )
		{
			MaximumSteps = int.MaxValue;
			CurrentStep = 0;
		}

		#endregion Commands

		#region Properties

		private BuildInformation _BuildInfo;
		private string _Changelog;
		private int _CurrentStep;
		private Version _CurrentVersion;
		private int _MaximumSteps;
		private string _ProgressText;
		private bool _ShouldBlogPost;
		private bool _ShouldCreateTag;
		private bool _ShouldPublish;
		private bool _ShouldUpload;
		private Version _TargetVersion;
		private bool _VersionOk;

		public BuildInformation BuildInfo
		{
			get
			{
				return _BuildInfo;
			}

			set
			{
				if( _BuildInfo == value )
				{
					return;
				}

				RaisePropertyChanging( () => BuildInfo );
				_BuildInfo = value;
				RaisePropertyChanged( () => BuildInfo );
			}
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

				RaisePropertyChanging( () => Changelog );
				_Changelog = value;
				RaisePropertyChanged( () => Changelog );
			}
		}

		public int CurrentStep
		{
			get
			{
				return _CurrentStep;
			}

			set
			{
				if( _CurrentStep == value )
				{
					return;
				}

				RaisePropertyChanging( () => CurrentStep );
				_CurrentStep = value;
				RaisePropertyChanged( () => CurrentStep );
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

		public int MaximumSteps
		{
			get
			{
				return _MaximumSteps;
			}

			set
			{
				if( _MaximumSteps == value )
				{
					return;
				}

				RaisePropertyChanging( () => MaximumSteps );
				_MaximumSteps = value;
				RaisePropertyChanged( () => MaximumSteps );
			}
		}

		public string ProgressText
		{
			get
			{
				return _ProgressText;
			}

			set
			{
				if( _ProgressText == value )
				{
					return;
				}

				RaisePropertyChanging( () => ProgressText );
				_ProgressText = value;
				RaisePropertyChanged( () => ProgressText );
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
		private BackgroundWorker Worker;

		#endregion Attributes
	}
}
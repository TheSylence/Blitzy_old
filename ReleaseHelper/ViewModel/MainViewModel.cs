using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Xml;
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

			TargetVersion = new Version( File.ReadAllText( "../../../Blitzy/Properties/Version.txt" ).Trim() );
			BuildInfo = new BuildInformation();

			TargetFileName = string.Format( "Blitzy.{0}.msi", TargetVersion );
			SourcePath = Path.Combine( BuildInfo.OutputPath, TargetFileName );

			MaximumSteps = int.MaxValue;
			ShouldCreateTag = true;
			ShouldPublish = true;
			ShouldUpload = true;
		}

		#endregion Constructor

		#region Methods

		private bool CreateNewWixProductID()
		{
			XmlDocument doc = new XmlDocument();
			WixNamespaceManager namespaceManager = new WixNamespaceManager( doc.NameTable );

			doc.Load( BuildInfo.WixFile );

			XmlNode node = doc.SelectSingleNode( "w:Wix/w:Product", namespaceManager );
			node.Attributes.GetNamedItem( "Id" ).Value = Guid.NewGuid().ToString();

			doc.PreserveWhitespace = true;
			doc.Save( BuildInfo.WixFile );

			return true;
		}

		private bool CreateTag()
		{
			ProcessStartInfo inf = new ProcessStartInfo();
			inf.FileName = BuildInfo.GitPath;
			inf.Arguments = "tag -a " + TargetVersion.ToString();

			Process proc = new Process();
			proc.StartInfo = inf;
			proc.Start();
			proc.WaitForExit();

			return proc.ExitCode == 0;
		}

		private bool PublishVersion()
		{
			Task<PublishResult> task = Api.PublishUpdate( Constants.ProjectName, TargetVersion, Changelog, TargetFileName, Constants.DownloadTitle );
			task.Wait();
			PublishResult result = task.Result;
			return result.Status == HttpStatusCode.OK;
		}

		private bool RunMSBuild()
		{
			ProcessStartInfo inf = new ProcessStartInfo();
			inf.FileName = BuildInfo.MSBuildPath;
			inf.Arguments = BuildInfo.BuildFile;

			Process proc = new Process();
			proc.StartInfo = inf;
			proc.Start();
			proc.WaitForExit();

			return proc.ExitCode == 0;
		}

		private bool UploadToFtp()
		{
			Task<FileUploadResult> task = Api.UploadFile( SourcePath, TargetFileName, new NetworkCredential( Constants.FTP.User, Constants.FTP.Password ) );
			task.Wait();
			FileUploadResult result = task.Result;

			return false;
		}

		private bool WriteBlogPost()
		{
			return false;
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
			Worker.WorkerReportsProgress = true;

			Worker.RunWorkerAsync();
		}

		private void Worker_DoWork( object sender, DoWorkEventArgs e )
		{
			DispatcherHelper.CheckBeginInvokeOnUI( () =>
			{
				MaximumSteps = 6;
			} );

			int step = 0;
			Worker.ReportProgress( step++, "Updating installer ID..." );
			if( !CreateNewWixProductID() )
			{
				e.Result = "Failed to update installer";
				return;
			}

			Worker.ReportProgress( step++, "Building solution..." );
			if( !RunMSBuild() )
			{
				e.Result = "Failed to build solution";
				return;
			}

			Worker.ReportProgress( step++, "Creating Tag..." );
			if( ShouldCreateTag )
			{
				if( !CreateTag() )
				{
					e.Result = "Failed to create tag";
					return;
				}
			}

			Worker.ReportProgress( step++, "Uploading to FTP..." );
			if( ShouldUpload )
			{
				if( !UploadToFtp() )
				{
					e.Result = "Failed to upload";
					return;
				}
			}

			Worker.ReportProgress( step++, "Publishing Version..." );
			if( ShouldPublish )
			{
				if( !PublishVersion() )
				{
					e.Result = "Failed to publish";
					return;
				}
			}

			Worker.ReportProgress( step++, "Writing Blog post..." );
			if( ShouldBlogPost )
			{
				if( !WriteBlogPost() )
				{
					e.Result = "Failed to write blog post";
					return;
				}
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
			ProgressText = "ERROR";

			if( e.Error != null )
			{
				MessageBox.Show( e.Error.ToString(), "Error", MessageBoxButton.OK, MessageBoxImage.Error );
				return;
			}

			if( e.Result != null )
			{
				MessageBox.Show( (string)e.Result, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
			}
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
		private string _SourcePath;
		private string _TargetFileName;

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

		public string SourcePath
		{
			get
			{
				return _SourcePath;
			}

			set
			{
				if( _SourcePath == value )
				{
					return;
				}

				RaisePropertyChanging( () => SourcePath );
				_SourcePath = value;
				RaisePropertyChanged( () => SourcePath );
			}
		}

		public string TargetFileName
		{
			get
			{
				return _TargetFileName;
			}

			set
			{
				if( _TargetFileName == value )
				{
					return;
				}

				RaisePropertyChanging( () => TargetFileName );
				_TargetFileName = value;
				RaisePropertyChanged( () => TargetFileName );
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
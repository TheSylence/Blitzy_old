// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

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

			GitPath = FindGit();

			string basePath = Environment.GetFolderPath( Environment.SpecialFolder.Windows );
			basePath = Path.Combine( basePath, "Microsoft.NET", "Framework" );
			string clrVersion = "v" + Environment.Version.ToString( 3 );

			MSBuildPath = Path.Combine( basePath, clrVersion, "MSBuild.exe" );
		}

		#endregion Constructor

		#region Methods

		private string FindGit()
		{
			string path = Environment.GetEnvironmentVariable( "Path", EnvironmentVariableTarget.Machine );
			string[] paths = path.Split( ';' );

			string folder = paths.Where( p => p.ToLower().Contains( @"git\cmd" ) ).FirstOrDefault();

			return Path.Combine( folder, "git.exe" );
		}

		#endregion Methods

		#region Properties

		private string _BuildFile;
		private string _GitPath;
		private string _MSBuildPath;
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

		public string GitPath
		{
			get
			{
				return _GitPath;
			}

			set
			{
				if( _GitPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => GitPath );
				_GitPath = value;
				RaisePropertyChanged( () => GitPath );
			}
		}

		public string MSBuildPath
		{
			get
			{
				return _MSBuildPath;
			}

			set
			{
				if( _MSBuildPath == value )
				{
					return;
				}

				RaisePropertyChanging( () => MSBuildPath );
				_MSBuildPath = value;
				RaisePropertyChanged( () => MSBuildPath );
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
}
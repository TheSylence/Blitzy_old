// $Id$

using System;
using System.Diagnostics;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Blitzy.btbapi;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Model
{
	internal class UpdateChecker
	{
		#region Constructor

		private UpdateChecker()
		{
			Messenger.Default.Register<BalloonActivatedMessage>( this, OnBalloonActivated );
			Messenger.Default.Register<DownloadStatusMessage>( this, MessageTokens.DownloadSucessful, OnSuccessfulDownload );
		}

		#endregion Constructor

		#region Methods

		internal async Task<VersionInfo> CheckVersion( bool showIfNewest = false )
		{
			// TODO: Set this to false for public release
			bool includePreReleases = true;

			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "Checking for updates..." );
			VersionInfo versionInfo = await API.CheckVersion( Constants.SoftwareName, currentVersion, includePreReleases );
			if( versionInfo.Status == HttpStatusCode.OK )
			{
				LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "Latest available version is {0}", versionInfo.LatestVersion );
				string downloadLink = null;
				if( versionInfo.DownloadLink != null )
				{
					downloadLink = versionInfo.DownloadLink.ToString();
				}

				Messenger.Default.Send( new VersionCheckMessage( currentVersion, versionInfo, showIfNewest ) );
			}
			else
			{
				LogHelper.LogWarning( MethodBase.GetCurrentMethod().DeclaringType, "Failed to retrieve latest version: {0}", versionInfo.Status );
			}

			return versionInfo;
		}

		internal void DownloadLatestVersion( VersionInfo info )
		{
			string ext = System.IO.Path.GetExtension( info.DownloadLink.AbsolutePath ).Substring( 1 );
			TargetPath = IOUtils.GetTempFileName( ext );

			DownloadServiceParameters args = new DownloadServiceParameters( info.DownloadLink, TargetPath, info.Size, info.MD5 );
			DialogServiceManager.Show<DownloadService>( args );
		}

		private void OnBalloonActivated( BalloonActivatedMessage msg )
		{
			VersionCheckMessage versionCheck = msg.Token as VersionCheckMessage;
			if( versionCheck != null )
			{
				DialogServiceManager.Show<ViewChangelogService>( versionCheck.VersionInfo );
			}
		}

		private void OnSuccessfulDownload( DownloadStatusMessage msg )
		{
			if( msg.TargetPath == TargetPath )
			{
				Process.Start( TargetPath );
			}
		}

		#endregion Methods

		#region Properties

		internal static UpdateChecker Instance
		{
			get { return _Instance ?? ( _Instance = new UpdateChecker() ); }
		}

		private API API
		{
			get
			{
				if( !RuntimeConfig.Tests )
				{
					return new btbapi.API( APIEndPoint.Default );
				}

				return new API( APIEndPoint.Localhost );
			}
		}

		private static UpdateChecker _Instance;

		#endregion Properties

		#region Attributes

		private static string TargetPath;

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.ViewServices;
using btbapi;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Model
{
	internal class UpdateChecker
	{
		#region Constructor

		private UpdateChecker()
		{
			Messenger.Default.Register<BalloonActivatedMessage>( this, msg => OnBalloonActivated( msg ) );
			Messenger.Default.Register<DownloadStatusMessage>( this, MessageTokens.DownloadSucessful, msg => OnSuccessfulDownload( msg ) );
		}

		#endregion Constructor

		#region Methods

		internal async Task<VersionInfo> CheckVersion( bool showIfNewest = false )
		{
			Version currentVersion = Assembly.GetExecutingAssembly().GetName().Version;

			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Checking for updates..." );
			VersionInfo versionInfo = await API.CheckVersion( Constants.SoftwareName, currentVersion );
			if( versionInfo.Status == System.Net.HttpStatusCode.OK )
			{
				LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Latest available version is {0}", versionInfo.LatestVersion );
				string downloadLink = null;
				if( versionInfo.DownloadLink != null )
				{
					downloadLink = versionInfo.DownloadLink.ToString();
				}

				Messenger.Default.Send<VersionCheckMessage>( new VersionCheckMessage( currentVersion, versionInfo, showIfNewest ) );
			}
			else
			{
				LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "Failed to retrieve latest version: {0}", versionInfo.Status );
			}

			return versionInfo;
		}

		internal void DownloadLatestVersion( VersionInfo info )
		{
			TargetPath = IOUtils.GetTempFileName( "exe" );

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

		private static UpdateChecker _Instance;

		internal static UpdateChecker Instance
		{
			get
			{
				if( _Instance == null )
				{
					_Instance = new UpdateChecker();
				}

				return _Instance;
			}
		}

		private API API
		{
			get
			{
				if( !RuntimeConfig.Tests )
				{
#if DEBUG
					return new API( APIEndPoint.Localhost );
#else
				return new btbapi.API( APIEndPoint.Default );
#endif
				}
				else
				{
					return new API( APIEndPoint.Localhost );
				}
			}
		}

		#endregion Properties

		#region Attributes

		private static string TargetPath;

		#endregion Attributes
	}
}
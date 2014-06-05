// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using btbapi;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Model
{
	internal static class UpdateChecker
	{
		private static API API
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

		internal static async Task<VersionInfo> CheckVersion()
		{
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Checking for updates..." );
			VersionInfo versionInfo = await API.CheckVersion( Constants.SoftwareName, Assembly.GetExecutingAssembly().GetName().Version );
			if( versionInfo.Status == System.Net.HttpStatusCode.OK )
			{
				LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Latest available version is {0}", versionInfo.LatestVersion );
				string downloadLink = null;
				if( versionInfo.DownloadLink != null )
				{
					downloadLink = versionInfo.DownloadLink.ToString();
				}
				Messenger.Default.Send<VersionCheckMessage>( new VersionCheckMessage( versionInfo.LatestVersion, downloadLink, true ) );
			}
			else
			{
				LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "Failed to retrieve latest version: {0}", versionInfo.Status );
			}

			return versionInfo;
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Model
{
	internal class UpdateChecker : BaseObject, IDisposable
	{
		#region Constructor

		public UpdateChecker()
		{
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Properties

		public void StartCheck( bool showIfNewest )
		{
			LogInfo( "Checking for new version..." );
			ShowIfNewest = showIfNewest;

			string url = Constants.UpdateCheckURL;

			Client = ToDispose( new WebClient() );
			Client.DownloadStringCompleted += new DownloadStringCompletedEventHandler( client_DownloadStringCompleted );
			Client.DownloadStringAsync( new Uri( url ) );
		}

		private void client_DownloadStringCompleted( object sender, DownloadStringCompletedEventArgs e )
		{
			if( e.Error != null )
			{
				LogError( "Failed to retrieve newest version: {0}", e.Error.ToString() );
				return;
			}
			else
			{
				string[] lines = e.Result.Split( '\n' );
				LogInfo( "Newest version is {0}", lines[0] );

				Version v;
				if( !Version.TryParse( lines[0], out v ) )
				{
					LogError( "Received invalid version info: {0}", lines[0] );
					return;
				}

				Messenger.Default.Send<VersionCheckMessage>( new VersionCheckMessage( v, lines[1], ShowIfNewest ) );
			}
		}

		#endregion Properties

		#region Attributes

		private WebClient Client;
		private bool ShowIfNewest;

		#endregion Attributes
	}
}
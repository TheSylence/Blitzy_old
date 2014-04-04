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
	internal class UpdateChecker : LogObject, IDisposable
	{
		#region Constructor

		public UpdateChecker()
		{
		}

		#endregion Constructor

		#region Disposable

		/// <summary>
		/// Releases unmanaged resources and performs other cleanup operations before the
		/// <see cref="Renderer"/> is reclaimed by garbage collection.
		/// </summary>
		~UpdateChecker()
		{
			Dispose( false );
		}

		/// <summary>
		/// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
		/// </summary>
		public void Dispose()
		{
			Dispose( true );
			GC.SuppressFinalize( this );
		}

		/// <summary>
		/// Releases unmanaged and - optionally - managed resources
		/// </summary>
		/// <param name="managed"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
		protected void Dispose( bool managed )
		{
			if( managed )
			{
				if( Client != null )
				{
					Client.Dispose();
				}
			}
		}

		#endregion Disposable

		#region Methods

		#endregion Methods

		#region Properties

		public void StartCheck( bool showIfNewest )
		{
			LogInfo( "Checking for new version..." );
			ShowIfNewest = showIfNewest;

			string url = Constants.UpdateCheckURL;

			Client = new WebClient();
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
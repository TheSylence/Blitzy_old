// $Id$

using System.Diagnostics.CodeAnalysis;
using Blitzy.Messages;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace Blitzy.Controls
{
	[ExcludeFromCodeCoverage]
	internal class TrayIcon : TaskbarIcon
	{
		#region Constructor

		public TrayIcon()
		{
			Messenger.Default.Register<CommandMessage>( this, msg => OnCommand( msg ) );
			Messenger.Default.Register<BalloonTipMessage>( this, msg => OnBallon( msg ) );

			this.TrayBalloonTipClicked += TrayIcon_TrayBalloonTipClicked;
		}

		#endregion Constructor

		#region Methods

		private void OnBallon( BalloonTipMessage msg )
		{
			BallonToken = msg.Token;
			ShowBalloonTip( msg.Title, msg.Message, msg.Icon );
		}

		private void OnCommand( CommandMessage msg )
		{
			if( msg.Status == CommandStatus.Error )
			{
				ShowBalloonTip( "Error".Localize(), msg.Message, BalloonIcon.Error );
			}
		}

		private void TrayIcon_TrayBalloonTipClicked( object sender, System.Windows.RoutedEventArgs e )
		{
			Messenger.Default.Send( new BalloonActivatedMessage( BallonToken ) );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private object BallonToken;

		#endregion Attributes
	}
}
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.Messages;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Messaging;
using Hardcodet.Wpf.TaskbarNotification;

namespace Blitzy.Controls
{
	[ExcludeFromCodeCoverage]
	internal class TrayIcon : TaskbarIcon
	{
		public TrayIcon()
		{
			Messenger.Default.Register<CommandMessage>( this, OnCommand );
			Messenger.Default.Register<BalloonTipMessage>( this, OnBallon );

			TrayBalloonTipClicked += TrayIcon_TrayBalloonTipClicked;
		}

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

		private void TrayIcon_TrayBalloonTipClicked( object sender, RoutedEventArgs e )
		{
			Messenger.Default.Send( new BalloonActivatedMessage( BallonToken ) );
		}

		#endregion Methods

		#region Attributes

		private object BallonToken;

		#endregion Attributes
	}
}
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
		}

		#endregion Constructor

		#region Methods

		private void OnBallon( BalloonTipMessage msg )
		{
			ShowBalloonTip( msg.Title, msg.Message, msg.Icon );
		}

		private void OnCommand( CommandMessage msg )
		{
			if( msg.Status == CommandStatus.Error )
			{
				ShowBalloonTip( "Error".Localize(), msg.Message, BalloonIcon.Error );
			}
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Messages;
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
		}

		#endregion Constructor

		#region Methods

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
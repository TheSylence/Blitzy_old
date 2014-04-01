// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	internal class MessageBoxParameter
	{
		#region Constructor

		public MessageBoxParameter( string text, string caption, MessageBoxButton button = MessageBoxButton.YesNo, MessageBoxImage icon = MessageBoxImage.Question )
		{
			Text = text;
			Caption = caption;
			Button = button;
			Icon = icon;
		}

		#endregion Constructor

		#region Properties

		public MessageBoxButton Button { get; set; }

		public string Caption { get; set; }

		public MessageBoxImage Icon { get; set; }

		public string Text { get; set; }

		#endregion Properties
	}
}
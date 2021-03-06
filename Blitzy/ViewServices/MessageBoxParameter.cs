﻿using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	public class MessageBoxParameter
	{
		public MessageBoxParameter( string text, string caption, MessageBoxButton button = MessageBoxButton.YesNo, MessageBoxImage icon = MessageBoxImage.Question )
		{
			Text = text;
			Caption = caption;
			Button = button;
			Icon = icon;
		}

		public MessageBoxButton Button { get; set; }

		public string Caption { get; set; }

		public MessageBoxImage Icon { get; set; }

		public string Text { get; set; }
	}
}
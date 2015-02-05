

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Messages
{
	internal enum PluginAction
	{
		Enabled,
		Disabled
	}

	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class PluginMessage : MessageBase
	{
		public readonly PluginAction Action;

		public readonly IPlugin Plugin;

		public PluginMessage( IPlugin plugin, PluginAction action )
		{
			Plugin = plugin;
			Action = action;
		}
	}
}
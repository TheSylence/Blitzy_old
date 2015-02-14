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
		public PluginMessage( IPlugin plugin, PluginAction action )
		{
			Plugin = plugin;
			Action = action;
		}

		public readonly PluginAction Action;

		public readonly IPlugin Plugin;
	}
}
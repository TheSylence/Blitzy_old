using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Plugin.SystemPlugins
{
	internal abstract class InternalPlugin : BaseObject, IPlugin
	{
		public abstract void ClearCache();

		public abstract bool ExecuteCommand( Model.CommandItem command, CommandExecutionMode mode, IList<string> input, out string message );

		public abstract IEnumerable<Model.CommandItem> GetCommands( IList<string> input );

		public abstract string GetInfo( IList<string> data, Model.CommandItem item );

		public abstract IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices );

		public abstract System.Windows.Controls.Control GetSettingsUI();

		public abstract IEnumerable<Model.CommandItem> GetSubCommands( Model.CommandItem parent, IList<string> input );

		public abstract bool Load( IPluginHost host, string oldVersion = null );

		public abstract void Unload( PluginUnloadReason reason );

		public abstract int ApiVersion { get; }

		public abstract string Author { get; }

		public abstract string Description { get; }

		public abstract bool HasSettings { get; }

		public abstract string Name { get; }

		public abstract Guid PluginID { get; }

		public abstract string Version { get; }

		public abstract Uri Website { get; }
	}
}
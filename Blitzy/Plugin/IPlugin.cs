using System;
using System.Collections.Generic;
using System.Windows.Controls;
using Blitzy.Model;

namespace Blitzy.Plugin
{
	/// <summary>
	/// Modes a command's execution can be requested in
	/// </summary>
	public enum CommandExecutionMode
	{
		/// <summary>
		/// Default mode
		/// </summary>
		Default,

		/// <summary>
		/// Secondary mode for less often used actions
		/// </summary>
		Secondary
	}

	/// <summary>
	/// Reason why a plugin is unloaded
	/// </summary>
	public enum PluginUnloadReason
	{
		/// <summary>
		/// The application is shutting down
		/// </summary>
		Shutdown,

		/// <summary>
		/// The plugin is disabled by the user
		/// </summary>
		Unload
	}

	/// <summary>
	/// Interface for a plugin. Implement this to build your plugin
	/// </summary>
	public interface IPlugin
	{
		/// <summary>
		/// Called when the plugin's cache (if there is any) should be cleared. This happens when the catalog is being rebuild for example.
		/// </summary>
		/// <remarks>It is totally acceptable if you recreate objects during this call.
		/// However you should only do so if the recreation is fast enough and does not consme too much memory.</remarks>
		void ClearCache();

		/// <summary>
		/// Executes a command.
		/// </summary>
		/// <param name="command">The selected command that should be executed</param>
		/// <param name="mode">The <see cref="CommandExecutionMode"/> this command should be executed in</param>
		/// <param name="input">The currently entered command list</param>
		/// <param name="message">A message that should be shown the user if the execution failed.</param>
		/// <returns><c>true</c> if the command was executed successfully, otherwise <c>false</c></returns>
		bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message );

		/// <summary>
		/// Get all commands this plugin can execute.
		/// You may return different items based upon <paramref name="input"/>
		/// </summary>
		/// <param name="input">The currently entered command list</param>
		/// <returns>A list of all commands the plugins provides.</returns>
		IEnumerable<CommandItem> GetCommands( IList<string> input );

		/// <summary>
		/// Gets information about the currently active command.
		/// </summary>
		/// <param name="data">The currently entered command list.</param>
		/// <param name="item">The currently selected command item.</param>
		/// <returns>The information that should be displayed to the user.
		/// Return <c>null</c> when you don't want to show any information.</returns>
		string GetInfo( IList<string> data, CommandItem item );

		/// <summary>
		/// Called along with <see cref="GetSettingsUI"/> to get access to the ViewModel for this plugin's settings
		/// </summary>
		/// <returns></returns>
		IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices );

		/// <summary>
		/// Called when the SettingsDialog is about to show the settings for this Plugin.
		/// </summary>
		/// <remarks>This method won't be called if <see cref="HasSettings"/> is <c>false</c>.
		/// Note: The DataContext of the returned control will be set to <see cref="GetSettingsDataContext"/>
		/// by the application.</remarks>
		/// <returns>The UI for the settings to display.</returns>
		Control GetSettingsUI();

		/// <summary>
		/// Get all sub commands the given command offers.
		/// You may return different results based upon <paramref name="input"/>
		/// </summary>
		/// <param name="parent">The command whose sub commands are queried</param>
		/// <param name="input">The currently entered command list.</param>
		/// <returns>A list of all sub commands the given command offers.</returns>
		IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input );

		/// <summary>
		/// Called when the plugin is loaded.
		/// </summary>
		/// <remarks>
		/// This method is the first method that will ever be called on a plugin.
		/// However properties may be accessed before a call to this method has been made.
		/// </remarks>
		/// <param name="host">The plugin's host.</param>
		/// <param name="oldVersion">If this value is <c>null</c> this is the first time ever your plugin is loaded.
		/// If it is not this will be the last version your plugin was started with. You can use this value for upgrading.</param>
		/// <returns><c>true</c> if the initialization of the plugin was successful, otherwise <c>false</c></returns>
		bool Load( IPluginHost host, string oldVersion = null );

		/// <summary>
		/// Called when the plugin is being unloaded
		/// </summary>
		/// <param name="reason">The reason why the plugin is being unloaded</param>
		void Unload( PluginUnloadReason reason );

		/// <summary>
		/// Gets the API version this plugin is built against.
		/// </summary>
		int ApiVersion { get; }

		/// <summary>
		/// Gets the name of the author of the plugin. This value is displayed in the settings
		/// </summary>
		string Author { get; }

		/// <summary>
		/// Gets a short description of the plugin.
		/// </summary>
		string Description { get; }

		/// <summary>
		/// Gets a flag whether this plugin offers configurable settings for the user.
		/// </summary>
		bool HasSettings { get; }

		/// <summary>
		/// Gets the name of the plugin.
		/// </summary>
		string Name { get; }

		Guid PluginID { get; }

		/// <summary>
		/// Gets the version of the plugin.
		/// </summary>
		string Version { get; }

		/// <summary>
		/// Gets the website of the author of the plugin. This value is linked in the settings.
		/// </summary>
		Uri Website { get; }
	}
}
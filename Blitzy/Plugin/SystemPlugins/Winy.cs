// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Blitzy.Model;
using Blitzy.Utility;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Winy : IPlugin
	{
		public void ClearCache()
		{
			Confirmations = new Dictionary<string, bool>
			{
				{"shutdown", Settings.GetValue<bool>(this, ShutdownKey)},
				{"restart", Settings.GetValue<bool>(this, RestartKey)},
				{"logoff", Settings.GetValue<bool>(this, LogoffKey)}
			};
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			message = null;
			if( Confirmations[command.Name] )
			{
				MessageBoxParameter mbArgs = new MessageBoxParameter( "ConfirmOperation".Localize(), "ConfirmationRequired".Localize() );
				MessageBoxResult result = MessageBoxResult.No;

				DispatcherHelper.RunAsync( () => result = ViewServiceManager.Default.Show<MessageBoxService, MessageBoxResult>( mbArgs ) ).Wait();
				if( result == MessageBoxResult.No )
				{
					return true;
				}
			}

			string cmd = string.Empty;
			string args = string.Empty;

			switch( command.Name )
			{
				case "shutdown":
					cmd = "shutdown";
					args = "-s -t 00";
					break;

				case "restart":
					cmd = "shutdown";
					args = "-r -t 00";
					break;

				case "logoff":
					cmd = "logoff";
					break;
			}

			Process.Start( cmd, args );
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "shutdown", "Shutdown".Localize(), this );
			yield return CommandItem.Create( "restart", "Restart".Localize(), this );
			yield return CommandItem.Create( "logoff", "Logoff".Localize(), this );
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return new ViewModel.WinySettingsViewModel( (Settings)Settings, viewServices );
		}

		public System.Windows.Controls.Control GetSettingsUI()
		{
			return new WinyUI();
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield break;
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Settings = host.Settings;
			if( oldVersion == null )
			{
				SetDefaultSettings( Settings );
			}

			ClearCache();
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
		}

		internal void SetDefaultSettings( ISettings settings )
		{
			settings.SetValue( this, LogoffKey, true );
			settings.SetValue( this, ShutdownKey, true );
			settings.SetValue( this, RestartKey, true );
		}

		public int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Provides a set of functions to control Windows"; }
		}

		public bool HasSettings { get { return true; } }

		public string Name
		{
			get { return "Winy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( GuidString );
				}

				return Guid.Value;
			}
		}

		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		internal const string GuidString = "26F7306C-AF81-4979-9F5B-1857EB9387BF";
		internal const string LogoffKey = "ConfirmLogoff";
		internal const string RestartKey = "ConfirmRestart";
		internal const string ShutdownKey = "ConfirmShutdown";
		private Dictionary<string, bool> Confirmations;
		private Guid? Guid;
		private ISettings Settings;
	}
}
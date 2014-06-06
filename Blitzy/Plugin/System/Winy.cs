// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.Windows;
using Blitzy.Model;
using Blitzy.Utility;
using Blitzy.ViewServices;

namespace Blitzy.Plugin.System
{
	internal class Winy : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			Confirmations = new Dictionary<string, bool>();
			Confirmations.Add( "shutdown", Settings.GetValue<bool>( this, ShutdownKey ) );
			Confirmations.Add( "restart", Settings.GetValue<bool>( this, RestartKey ) );
			Confirmations.Add( "logoff", Settings.GetValue<bool>( this, LogoffKey ) );
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			message = null;
			if( Confirmations[command.Name] )
			{
				MessageBoxParameter mbArgs = new MessageBoxParameter( "ConfirmOperation".Localize(), "ConfirmationRequired".Localize() );
				MessageBoxResult result = DialogServiceManager.Show<MessageBoxService, MessageBoxResult>( mbArgs );
				return true;
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

		public IEnumerable<Model.CommandItem> GetCommands( IList<string> input )
		{
			yield return CommandItem.Create( "shutdown", "Shutdown".Localize(), this );
			yield return CommandItem.Create( "restart", "Restart".Localize(), this );
			yield return CommandItem.Create( "logoff", "Logoff".Localize(), this );
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
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

		#endregion Methods

		#region Constants

		internal const string GuidString = "26F7306C-AF81-4979-9F5B-1857EB9387BF";
		internal const string LogoffKey = "ConfirmLogoff";
		internal const string RestartKey = "ConfirmRestart";
		internal const string ShutdownKey = "ConfirmShutdown";

		#endregion Constants

		#region Properties

		private Guid? GUID;

		public int ApiVersion
		{
			get { return Constants.APIVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Provides a set of functions to control Windows"; }
		}

		public string Name
		{
			get { return "Winy"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( GuidString );
				}

				return GUID.Value;
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

		#endregion Properties

		#region Attributes

		private Dictionary<string, bool> Confirmations;
		private ISettings Settings;

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Model;
using Blitzy.ViewServices;

namespace Blitzy.Plugin.System
{
	internal class Winy : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			Confirmations = new Dictionary<string, bool>();
			Confirmations.Add( "shutdown", Settings.GetSystemSetting<bool>( SystemSetting.ConfirmShutdown ) );
			Confirmations.Add( "restart", Settings.GetSystemSetting<bool>( SystemSetting.ConfirmRestart ) );
			Confirmations.Add( "logoff", Settings.GetSystemSetting<bool>( SystemSetting.ConfirmLogoff ) );
		}

		public bool ExecuteCommand( CommandItem command, IList<string> input, out string message )
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
			return Enumerable.Empty<CommandItem>();
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Settings = host.Settings;
			ClearCache();
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
		}

		#endregion Methods

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
					GUID = Guid.Parse( "26F7306C-AF81-4979-9F5B-1857EB9387BF" );
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
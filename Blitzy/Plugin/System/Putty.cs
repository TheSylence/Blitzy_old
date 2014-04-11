// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Microsoft.Win32;

namespace Blitzy.Plugin.System
{
	internal class Putty : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			RootItem = CommandItem.Create( "ssh", "Connect to a remote host using putty", this, "", false, null, new[] { "putty" } );
		}

		public bool ExecuteCommand( CommandItem command, IList<string> input, out string message )
		{
			// Session loading: putty -load "session name"
			// Connection: putty [user@]host
			bool session = (bool)command.UserData;

			if( session )
			{
				Process.Start( PuttyPath, string.Format( CultureInfo.InvariantCulture, "-load \"{0}\"", command.Name ) );
			}
			else
			{
				if( input.Count > 0 )
				{
					Process.Start( PuttyPath, input[0] );
				}
				else
				{
					Process.Start( PuttyPath );
				}
			}

			message = null;
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			yield return RootItem;
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			if( Host.Settings.GetSystemSetting<bool>( SystemSetting.ImportPuttySessions ) )
			{
				using( RegistryKey puttyKey = Registry.CurrentUser.OpenSubKey( @"Software\SimonTatham\PuTTY\Sessions" ) )
				{
					if( puttyKey != null )
					{
						foreach( string str in puttyKey.GetSubKeyNames().OrderBy( s => s.GetDiceCoefficent( input[0] ) ) )
						{
							yield return CommandItem.Create( str, string.Format( CultureInfo.CurrentUICulture, "Open saved session '{0}'", str ), this, "", true, RootItem );
						}
					}
				}
			}
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;
			ClearCache();
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Properties

		private Guid? GUID;
		private IPluginHost Host;

		private CommandItem RootItem;

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
			get { return "Open putty or directly connect to remote hosts"; }
		}

		public string Name
		{
			get { return "Putty"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( "9FF8854A-68AB-4586-BB1A-03061A270C84" );
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

		private string PuttyPath
		{
			get
			{
				return Host.Settings.GetSystemSetting<string>( Model.SystemSetting.PuttyPath );
			}
		}

		#endregion Properties
	}
}
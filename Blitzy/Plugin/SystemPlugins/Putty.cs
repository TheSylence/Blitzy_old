using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using Blitzy.Model;
using Blitzy.Utility;
using Microsoft.Win32;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Putty : InternalPlugin
	{
		public override void ClearCache()
		{
			CreateCommands();
		}

		public override bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
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

		public override IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( RootItem == null )
			{
				CreateCommands();
			}

			yield return RootItem;
		}

		public override string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public override IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return new ViewModel.PuttySettingsViewModel( Host.ConnectionFactory, (Settings)Host.Settings, viewServices );
		}

		public override System.Windows.Controls.Control GetSettingsUI()
		{
			return new PuttyUI();
		}

		public override IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			if( parent == RootItem )
			{
				if( Host.Settings.GetValue<bool>( this, ImportKey ) )
				{
					using( RegistryKey puttyKey = Registry.CurrentUser.OpenSubKey( @"Software\SimonTatham\PuTTY\Sessions" ) )
					{
						if( puttyKey != null )
						{
							foreach( string str in puttyKey.GetSubKeyNames().OrderBy( s => s.GetDiceCoefficent( input[0] ) ) )
							{
								yield return CommandItem.Create( str, string.Format( CultureInfo.CurrentUICulture, "Open saved session '{0}'", str ), this, PuttyPath, true, RootItem );
							}
						}
					}
				}
			}
		}

		public override bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;
			if( oldVersion == null )
			{
				SetDefaultValues( host.Settings );
			}

			return true;
		}

		public override void Unload( PluginUnloadReason reason )
		{
		}

		internal void SetDefaultValues( ISettings settings )
		{
			settings.SetValue( this, ImportKey, false );
			settings.SetValue( this, PathKey, string.Empty );
		}

		private void CreateCommands()
		{
			RootItem = CommandItem.Create( "ssh", "PuttyDescription".Localize(), this, PuttyPath, false, null, new[] { "putty" } );
		}

		public override int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public override string Author
		{
			get { return "Matthias Specht"; }
		}

		public override string Description
		{
			get { return "Open putty or directly connect to remote hosts"; }
		}

		public override bool HasSettings { get { return true; } }

		public override string Name
		{
			get { return "Putty"; }
		}

		public override Guid PluginID
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

		public override string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public override Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		private string PuttyPath
		{
			get
			{
				return Host.Settings.GetValue<string>( this, PathKey );
			}
		}

		internal const string GuidString = "9FF8854A-68AB-4586-BB1A-03061A270C84";
		internal const string ImportKey = "ImportPuttySessions";
		internal const string PathKey = "PuttyPath";

		private Guid? Guid;
		private IPluginHost Host;
		private CommandItem RootItem;
	}
}
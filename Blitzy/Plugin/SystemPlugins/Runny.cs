using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Blitzy.Model;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Runny : InternalPlugin, ISystemPlugin
	{
		public override void ClearCache()
		{
			foreach( CommandItem item in ItemCache )
			{
				IDisposable disp = item.UserData as IDisposable;
				if( disp != null )
				{
					DisposeObject( disp );
				}
			}

			ItemCache.Clear();
		}

		public override bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			string args;
			string workingDirectory;

			Workspace workspace = command.UserData as Workspace;
			if( workspace != null )
			{
				//workspace.Load( ( (Settings)Host.Settings ).Connection );

				foreach( WorkspaceItem item in workspace.Items.OrderBy( it => it.ItemOrder ) )
				{
					args = command.UserData as string;
					workingDirectory = Path.GetDirectoryName( item.ItemCommand );
					Run( item.ItemCommand, null, workingDirectory, mode == CommandExecutionMode.Secondary );
				}

				message = null;
				return true;
			}

			args = command.UserData as string;
			workingDirectory = Path.GetDirectoryName( command.Description );
			Run( command.Description, args, workingDirectory, mode == CommandExecutionMode.Secondary );

			message = null;
			return true;
		}

		public override IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( ItemCache.Count == 0 )
			{
				using( DbConnection connection = Host.ConnectionFactory.OpenConnection() )
				{
					ItemCache = new List<CommandItem>( ReadAllCommands( connection ) );
				}
			}

			return ItemCache;
		}

		public override string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public override IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return null;
		}

		public override System.Windows.Controls.Control GetSettingsUI()
		{
			return null;
		}

		public override IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield break;
		}

		public override bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;
			return true;
		}

		public override void Unload( PluginUnloadReason reason )
		{
			ItemCache.Clear();
		}

		internal IEnumerable<CommandItem> ReadAllCommands( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Command, Name, Icon, Arguments FROM files";

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						string command = reader.GetString( 0 );
						string name = reader.GetString( 1 );
						string icon = reader.GetString( 2 );
						string args = reader.GetString( 3 );

						yield return CommandItem.Create( name, command, this, icon, args );
					}
				}
			}

			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WorkspaceID FROM workspaces";

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Workspace workspace = ToDispose( new Workspace
						{
							ID = reader.GetInt32( 0 )
						} );

						workspace.Load( connection );

						yield return CommandItem.Create( workspace.Name, workspace.Name, this, "Workspace.png", workspace );
					}
				}
			}
		}

		private void Run( string fileName, string args, string workingDir, bool asAdmin )
		{
			ProcessStartInfo procInf = new ProcessStartInfo
			{
				FileName = fileName
			};

			if( workingDir != null )
			{
				procInf.WorkingDirectory = workingDir;
			}
			if( args != null )
			{
				procInf.Arguments = args;
			}
			if( asAdmin )
			{
				procInf.UseShellExecute = true;
				procInf.Verb = "runas";
			}

			Process.Start( procInf );
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
			get { return "Execution of files"; }
		}

		public override bool HasSettings { get { return false; } }

		public override string Name
		{
			get { return "Runny"; }
		}

		public override Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( "A41E22B3-52D0-42ED-8E27-76404108E393" );
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

		private Guid? Guid;
		private IPluginHost Host;
		private List<CommandItem> ItemCache = new List<CommandItem>();
	}
}
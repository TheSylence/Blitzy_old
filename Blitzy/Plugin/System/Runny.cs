// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Blitzy.Model;

namespace Blitzy.Plugin.System
{
	internal class Runny : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			ItemCache.Clear();
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			ProcessStartInfo procInf = new ProcessStartInfo
			{
				FileName = command.Description
			};

			Workspace workspace = command.UserData as Workspace;
			if( workspace != null )
			{
				// TODO: Execute workspace items
				message = null;
				return true;
			}

			string args = command.UserData as string;
			if( args != null )
			{
				procInf.Arguments = args;
			}

			if( mode == CommandExecutionMode.Secondary )
			{
				procInf.UseShellExecute = true;
				procInf.Verb = "runas";
			}

			string workingDirectory = Path.GetDirectoryName( command.Description );
			if( workingDirectory != null )
			{
				procInf.WorkingDirectory = workingDirectory;
			}

			Process.Start( procInf );

			message = null;
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( ItemCache.Count == 0 )
			{
				// TODO: Would be great if this was possible without bypassing the plugin API
				SQLiteConnection connection = ( (Settings)Host.Settings ).Connection;

				ItemCache = new List<CommandItem>( ReadAllCommands( connection ) );
			}

			return ItemCache;
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
			Host = host;
			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
			ItemCache.Clear();
		}

		internal IEnumerable<CommandItem> ReadAllCommands( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Command, Name, Icon, Arguments FROM files";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
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

			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WorkspaceID FROM workspaces";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Workspace workspace = new Workspace
						{
							ID = reader.GetInt32( 0 )
						};

						workspace.Load( connection );

						yield return CommandItem.Create( workspace.Name, workspace.Name, this, "Workspace.png", workspace );
					}
				}
			}
		}

		#endregion Methods

		#region Properties

		private Guid? Guid;

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
			get { return "Execution of files"; }
		}

		public string Name
		{
			get { return "Runny"; }
		}

		public Guid PluginID
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

		private IPluginHost Host;
		private List<CommandItem> ItemCache = new List<CommandItem>();

		#endregion Attributes
	}
}
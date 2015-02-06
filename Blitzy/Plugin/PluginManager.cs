using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;
using Blitzy.Messages;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Plugin
{
	internal class PluginManager : BaseObject
	{
		public PluginManager( IPluginHost host, DbConnectionFactory factory, IMessenger messenger = null )
		{
			MessengerInstance = messenger ?? Messenger.Default;
			Factory = factory;
			Host = host;

			MessengerInstance.Register<PluginMessage>( this, HandlePluginAction );
		}

		public bool IsLoaded( Guid id )
		{
			return Plugins.Exists( p => p.PluginID.Equals( id ) );
		}

		public void LoadPlugins()
		{
			string pluginsDir = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), Constants.PluginsFolderName );

			IEnumerable<string> files = new[] { Assembly.GetExecutingAssembly().Location };
			if( Directory.Exists( pluginsDir ) )
			{
				files = files.Concat( Directory.GetFiles( pluginsDir, "*.dll" ) );
			}
			else
			{
				LogWarning( "Plugin directory does not exist" );
			}

			foreach( string file in files )
			{
				LoadPluginsFromAssembly( file );
			}

			LogInfo( "Loaded {0} plugins and {1} disabled plugins", Plugins.Count, DisabledPlugins.Count );
		}

		internal void ClearCache()
		{
			foreach( IPlugin plugin in Plugins )
			{
				try
				{
					plugin.ClearCache();
				}
				catch( Exception ex )
				{
					LogWarning( "Failed to clear cache of plugin {0}: {1}", plugin.Name, ex );
				}
			}
		}

		internal IPlugin GetPlugin( Guid id )
		{
			return Plugins.Concat( DisabledPlugins ).Where( p => p.PluginID.Equals( id ) ).FirstOrDefault();
		}

		internal IPlugin GetPlugin( string name )
		{
			return Plugins.Concat( DisabledPlugins ).Where( p => p.Name.Equals( name ) ).FirstOrDefault();
		}

		internal void InstallPlugin( string path )
		{
			LogInfo( "Installing plugin from {0}", path );
			string ext = Path.GetExtension( path );

			if( ext.Equals( ".dll", StringComparison.OrdinalIgnoreCase ) )
			{
				File.Copy( path, Path.Combine( Constants.PluginPath, Path.GetFileName( path ) ) );
			}
			else if( ext.Equals( ".zip", StringComparison.OrdinalIgnoreCase ) )
			{
				const string metaFileName = "plugin.pkg";
				using( ZipArchive archive = ZipFile.OpenRead( path ) )
				{
					ZipArchiveEntry metaEntry = archive.Entries.FirstOrDefault( e => e.FullName.Equals( metaFileName, StringComparison.OrdinalIgnoreCase ) );
					if( metaEntry == null )
					{
						LogWarning( "Failed to install plugin. No plugin.pkg found inside {0}", path );
						return;
					}

					using( TextReader reader = new StreamReader( metaEntry.Open() ) )
					{
						path = Path.Combine( Constants.PluginPath, reader.ReadLine().Trim() );
					}

					foreach( ZipArchiveEntry entry in archive.Entries.Where( e => !e.FullName.Equals( metaFileName, StringComparison.OrdinalIgnoreCase ) ) )
					{
						entry.ExtractToFile( Path.Combine( Constants.PluginPath, entry.FullName ) );
					}
				}
			}
			else
			{
				LogWarning( "Failed to install plugin. Unsupported file format" );
				return;
			}

			LoadPluginsFromAssembly( path );
		}

		private string GetLastInstalledPluginVersion( IPlugin plugin )
		{
			string version = null;
			bool disabled = false;

			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "pluginID", plugin.PluginID );

					cmd.CommandText = "SELECT Version, Disabled FROM plugins WHERE PluginID = @pluginID;";
					cmd.Prepare();

					using( DbDataReader reader = cmd.ExecuteReader() )
					{
						if( reader.Read() )
						{
							version = reader.GetString( 0 );
							disabled = reader.GetInt32( 1 ) == 1;
						}
					}
				}

				if( disabled )
				{
					LogInfo( "Plugin {0} was disabled by the user", plugin.Name );
					DisabledPlugins.Add( plugin );
					return null;
				}

				if( version == null )
				{
					LogInfo( "Plugin {0} is started for the first time", plugin.Name );
					using( DbCommand cmd = connection.CreateCommand() )
					{
						cmd.AddParameter( "pluginID", plugin.PluginID );
						cmd.AddParameter( "version", plugin.Version );

						cmd.CommandText = "INSERT INTO plugins (PluginID, Version) VALUES (@pluginID, @version);";
						cmd.Prepare();

						cmd.ExecuteNonQuery();
					}
				}
				else if( !version.Equals( plugin.Version ) )
				{
					LogInfo( "Plugin {0} is updated from version {1} to {2}", plugin.Name, version, plugin.Version );

					using( DbCommand cmd = connection.CreateCommand() )
					{
						cmd.AddParameter( "pluginID", plugin.PluginID );
						cmd.AddParameter( "version", plugin.Version );

						cmd.CommandText = "UPDATE plugins SET Version = @version WHERE PluginID = @pluginID;";
						cmd.Prepare();

						cmd.ExecuteNonQuery();
					}
				}

				return version;
			}
		}

		private void HandlePluginAction( PluginMessage msg )
		{
			if( msg.Action == PluginAction.Enabled )
			{
				try
				{
					string version = GetLastInstalledPluginVersion( msg.Plugin );
					msg.Plugin.Load( Host, version );
				}
				catch( Exception ex )
				{
					LogError( "Failed to load plugin {0}: {1}", msg.Plugin.Name, ex );
				}

				DisabledPlugins.Remove( msg.Plugin );
				Plugins.Add( msg.Plugin );
			}
			else if( msg.Action == PluginAction.Disabled )
			{
				try
				{
					msg.Plugin.Unload( PluginUnloadReason.Unload );
				}
				catch( Exception ex )
				{
					LogWarning( "Failed to unload plugin {0} properly: {1}", msg.Plugin.Name, ex );
				}

				DisabledPlugins.Add( msg.Plugin );
				Plugins.Remove( msg.Plugin );
			}
		}

		private void LoadPlugin( Assembly asm, Type type )
		{
			try
			{
				IPlugin plugin = asm.CreateInstance( type.FullName, false ) as IPlugin;
				if( plugin == null )
				{
					LogWarning( "Could not create plugin from {0} for type {1}", asm.FullName, type.FullName );
					return;
				}

				if( plugin.ApiVersion != Constants.ApiVersion )
				{
					LogError( "Failed to load plugin because API Versions do not match. Current Version: {0}, Plugin Version: {1}", Constants.ApiVersion, plugin.ApiVersion );
					return;
				}

				string version = GetLastInstalledPluginVersion( plugin );
				if( plugin.Load( Host, version ) )
				{
					LogInfo( "Loaded plugin {0}", plugin.Name );
					Plugins.Add( plugin );
				}
				else
				{
					LogError( "Failed to load plugin {0}", plugin.Name );
				}
			}
			catch( Exception ex )
			{
				LogError( "Failed to load plugin: {0}", ex );
			}
		}

		private void LoadPluginsFromAssembly( string file )
		{
			Type interfaceFace = typeof( IPlugin );

			LogDebug( "Loading plugins from {0}...", file );

			Assembly asm = Assembly.LoadFrom( file );
			IEnumerable<Type> types = Enumerable.Empty<Type>();
			try
			{
				types = asm.GetTypes().Where( t => !t.IsAbstract && interfaceFace.IsAssignableFrom( t ) );
			}
			catch( Exception ex )
			{
				LogError( "Failed to load plugins from {0}: {1}", file, ex );
			}

			foreach( Type type in types )
			{
				LoadPlugin( asm, type );
			}
		}

		internal List<IPlugin> DisabledPlugins = new List<IPlugin>();
		internal List<IPlugin> Plugins = new List<IPlugin>();
		private readonly IPluginHost Host;
		private DbConnectionFactory Factory;
		private IMessenger MessengerInstance;
	}
}
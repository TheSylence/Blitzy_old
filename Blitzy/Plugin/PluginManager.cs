// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Blitzy.Plugin
{
	internal class PluginManager : BaseObject
	{
		#region Constructor

		public PluginManager( IPluginHost host, SQLiteConnection connection )
		{
			Connection = connection;
			Host = host;
		}

		#endregion Constructor

		#region Methods

		public bool IsLoaded( Guid id )
		{
			return Plugins.Exists( p => p.PluginID.Equals( id ) );
		}

		public void LoadPlugins()
		{
			string pluginsDir = Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), Constants.PluginsFolderName );
			Type interfaceFace = typeof( IPlugin );

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
				LogDebug( "Loading plugins from {0}...", file );

				Assembly asm = Assembly.LoadFrom( file );
				foreach( Type type in asm.GetTypes().Where( t => !t.IsAbstract && interfaceFace.IsAssignableFrom( t ) ) )
				{
					LoadPlugin( asm, type );
				}
			}

			LogInfo( "Loaded {0} plugins and {1} disabled plugins", Plugins.Count, DisabledPlugins.Count );
		}

		internal IPlugin GetPlugin( Guid id )
		{
			return Plugins.Concat( DisabledPlugins ).Where( p => p.PluginID.Equals( id ) ).FirstOrDefault();
		}

		internal IPlugin GetPlugin( string name )
		{
			return Plugins.Concat( DisabledPlugins ).Where( p => p.Name.Equals( name ) ).FirstOrDefault();
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

				string version = null;
				bool disabled = false;

				using( SQLiteCommand cmd = Connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "pluginID";
					param.Value = plugin.PluginID;
					cmd.Parameters.Add( param );

					cmd.CommandText = "SELECT Version, Disabled FROM plugins WHERE PluginID = @pluginID;";
					cmd.Prepare();

					using( SQLiteDataReader reader = cmd.ExecuteReader() )
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
					return;
				}

				if( version == null )
				{
					LogInfo( "Plugin {0} is started for the first time", plugin.Name );
					using( SQLiteCommand cmd = Connection.CreateCommand() )
					{
						SQLiteParameter param = cmd.CreateParameter();
						param.ParameterName = "pluginID";
						param.Value = plugin.PluginID;
						cmd.Parameters.Add( param );

						param = cmd.CreateParameter();
						param.ParameterName = "version";
						param.Value = plugin.Version;
						cmd.Parameters.Add( param );

						cmd.CommandText = "INSERT INTO plugins (PluginID, Version) VALUES (@pluginID, @version);";
						cmd.Prepare();

						cmd.ExecuteNonQuery();
					}
				}
				else if( !version.Equals( plugin.Version ) )
				{
					LogInfo( "Plugin {0} is updated from version {1} to {2}", plugin.Name, version, plugin.Version );

					using( SQLiteCommand cmd = Connection.CreateCommand() )
					{
						SQLiteParameter param = cmd.CreateParameter();
						param.ParameterName = "pluginID";
						param.Value = plugin.PluginID;
						cmd.Parameters.Add( param );

						param = cmd.CreateParameter();
						param.ParameterName = "version";
						param.Value = plugin.Version;
						cmd.Parameters.Add( param );

						cmd.CommandText = "UPDATE plugins SET Version = @version WHERE PluginID = @pluginID;";
						cmd.Prepare();

						cmd.ExecuteNonQuery();
					}
				}

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

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		internal List<IPlugin> DisabledPlugins = new List<IPlugin>();
		internal List<IPlugin> Plugins = new List<IPlugin>();
		private readonly SQLiteConnection Connection;
		private readonly IPluginHost Host;

		#endregion Attributes
	}
}
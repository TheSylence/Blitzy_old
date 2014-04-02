// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	internal class Settings : ObservableObject, Blitzy.Plugin.ISettings
	{
		#region Constructor

		public Settings( SQLiteConnection connection )
		{
			Connection = connection;
			Folders = new ObservableCollection<Folder>();
		}

		#endregion Constructor

		#region Methods

		#region ISettings

		T Plugin.ISettings.GetValue<T>( Plugin.IPlugin plugin, string key )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}
			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = plugin.PluginID;
				param.ParameterName = "pluginID";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = key;
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT [Value] FROM plugin_settings WHERE PluginID = @pluginID AND [Key] = @key;";
				cmd.Prepare();

				return (T)Convert.ChangeType( cmd.ExecuteScalar(), typeof( T ) );
			}
		}

		void Plugin.ISettings.RemoveValue( Plugin.IPlugin plugin, string key )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}

			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = plugin.PluginID;
				param.ParameterName = "pluginID";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = key;
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				cmd.CommandText = "DELETE FROM plugin_settings WHERE PluginID = @pluginID AND [Key] = @key;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		void Plugin.ISettings.SetValue( Plugin.IPlugin plugin, string key, object value )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}
			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			( (Plugin.ISettings)this ).RemoveValue( plugin, key );

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = plugin.PluginID;
				param.ParameterName = "pluginID";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = key;
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = value;
				param.ParameterName = "value";
				cmd.Parameters.Add( param );

				cmd.CommandText = "INSERT INTO plugin_settings (PluginID, [Key], [Value]) VALUES( @pluginID ,@key, @value );";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		#endregion ISettings

		internal void Load()
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT FolderID, Path, Recursive FROM folders";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Folder f = new Folder();
						f.ID = reader.GetInt32( 0 );
						f.Path = reader.GetString( 1 );
						f.IsRecursive = reader.GetInt32( 2 ) == 1;

						Folders.Add( f );
					}
				}
			}
		}

		internal void Save()
		{
		}

		internal void SetDefaults()
		{
			// TODO:
		}

		#endregion Methods

		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		#endregion Properties

		#region Attributes

		private SQLiteConnection Connection;

		#endregion Attributes
	}
}
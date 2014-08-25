// $Id$

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using Blitzy.Plugin;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	internal class Settings : ObservableObject, ISettings
	{
		#region Constructor

		public Settings( SQLiteConnection connection )
		{
			Connection = connection;
			Folders = new ObservableCollection<Folder>();
		}

		#endregion Constructor

		#region Methods

		public T GetValue<T>( SystemSetting setting )
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = setting.ToString();
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT [Value] FROM settings WHERE [Key] = @key;";
				cmd.Prepare();

				object value = cmd.ExecuteScalar();
				return ConvertValue<T>( value );
			}
		}

		internal T ConvertValue<T>( object value )
		{
			if( DBNull.Value.Equals( value ) || value == null )
			{
				return default( T );
			}

			Type targetType = typeof( T );
			if( targetType == typeof( bool ) )
			{
				if( value is int )
				{
					value = ( (int)value ) == 1;
				}
				else if( value is string )
				{
					value = Convert.ToInt32( value.ToString() ) == 1;
				}
			}

			return (T)Convert.ChangeType( value, targetType );
		}

		internal T GetPluginSetting<T>( string pluginID, string key )
		{
			return GetPluginSetting<T>( Guid.Parse( pluginID ), key );
		}

		internal T GetPluginSetting<T>( Guid pluginID, string key )
		{
			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = pluginID;
				param.ParameterName = "pluginID";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = key;
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT [Value] FROM plugin_settings WHERE PluginID = @pluginID AND [Key] = @key;";
				cmd.Prepare();

				object value = cmd.ExecuteScalar();
				return ConvertValue<T>( value );
			}
		}

		internal void Load()
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT FolderID FROM folders";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Folder f = new Folder { ID = reader.GetInt32( 0 ) };
						f.Load( Connection );

						Folders.Add( f );
					}
				}
			}
		}

		internal void RemovePluginSetting( Guid pluginId, string key )
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = pluginId;
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

		internal void Save()
		{
			foreach( Folder f in Folders )
			{
				f.Save( Connection );
			}
		}

		internal void SetDefaults()
		{
			Type type = typeof( SystemSetting );

			SQLiteTransaction transaction = Connection.BeginTransaction();
			try
			{
				foreach( SystemSetting setting in Enum.GetValues( type ) )
				{
					MemberInfo member = type.GetMember( setting.ToString() ).First();
					SetValue( setting, member.GetCustomAttribute<DefaultValueAttribute>().Value );
				}

				transaction.Commit();
			}
			catch( Exception ex )
			{
				LogHelper.LogError( MethodInfo.GetCurrentMethod().DeclaringType, "Failed to restore default values: {0}", ex );
				transaction.Rollback();
				throw;
			}
		}

		internal void SetPluginSetting( string pluginID, string key, object value )
		{
			SetPluginSetting( Guid.Parse( pluginID ), key, value );
		}

		internal void SetPluginSetting( Guid pluginID, string key, object value )
		{
			RemovePluginSetting( pluginID, key );

			if( value is bool )
			{
				value = ( (bool)value ) ? 1 : 0;
			}

			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = pluginID;
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

		#region ISettings

		T ISettings.GetSystemSetting<T>( SystemSetting setting )
		{
			return GetValue<T>( setting );
		}

		T ISettings.GetValue<T>( IPlugin plugin, string key )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}
			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			return GetPluginSetting<T>( plugin.PluginID, key );
		}

		void ISettings.RemoveValue( IPlugin plugin, string key )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}

			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			RemovePluginSetting( plugin.PluginID, key );
		}

		void ISettings.SetValue( IPlugin plugin, string key, object value )
		{
			if( plugin == null )
			{
				throw new ArgumentNullException( "plugin" );
			}
			if( string.IsNullOrWhiteSpace( key ) )
			{
				throw new ArgumentNullException( "key" );
			}

			SetPluginSetting( plugin.PluginID, key, value );
		}

		#endregion ISettings

		internal void SetValue( SystemSetting setting, object value )
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.Value = setting.ToString();
				param.ParameterName = "key";
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.Value = value;
				param.ParameterName = "value";
				cmd.Parameters.Add( param );

				cmd.CommandText = "UPDATE settings SET [Value] = @value WHERE [Key] = @key;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		#endregion Methods

		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		internal SQLiteConnection Connection { get; private set; }

		#endregion Properties
	}
}
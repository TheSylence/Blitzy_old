using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using Blitzy.Plugin;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	public enum SystemSetting
	{
		[DefaultValue( 1 )]
		AutoUpdate,

		[DefaultValue( 1 )]
		CloseOnEscape,

		[DefaultValue( 1 )]
		CloseAfterCommand,

		[DefaultValue( 1 )]
		CloseOnFocusLost,

		[DefaultValue( "Control+Alt, Space" )]
		Shortcut,

		[DefaultValue( 1 )]
		TrayIcon,

		[DefaultValue( 0 )]
		KeepCommand,

		[DefaultValue( 0 )]
		StayOnTop,

		[DefaultValue( 20 )]
		MaxMatchingItems,

		[DefaultValue( "en" )]
		Language,

		[DefaultValue( "Default" )]
		Skin,

		[DefaultValue( 20 )]
		HistoryCount,

		[DefaultValue( 60 )]
		AutoCatalogRebuild,

		[DefaultValue( 1 )]
		BackupShortcuts,

		[DefaultValue( 0 )]
		RebuildCatalogOnChanges,

		[DefaultValue( "2000-01-01 00:00:00" )]
		LastCatalogBuild
	}

	internal class Settings : ObservableObject, ISettings
	{
		public Settings( DbConnectionFactory factory )
		{
			Factory = factory;
			Folders = new ObservableCollection<Folder>();
		}

		public T GetValue<T>( SystemSetting setting )
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "key", setting.ToString() );

					cmd.CommandText = "SELECT [Value] FROM settings WHERE [Key] = @key;";
					cmd.Prepare();

					object value = cmd.ExecuteScalar();
					return ConvertValue<T>( value );
				}
			}
		}

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

			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "pluginID", pluginID );
					cmd.AddParameter( "key", key );

					cmd.CommandText = "SELECT [Value] FROM plugin_settings WHERE PluginID = @pluginID AND [Key] = @key;";
					cmd.Prepare();

					object value = cmd.ExecuteScalar();
					return ConvertValue<T>( value );
				}
			}
		}

		internal void Load()
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT FolderID FROM folders";

					using( DbDataReader reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							Folder f = new Folder { ID = reader.GetInt32( 0 ) };
							f.Load( connection );

							Folders.Add( f );
						}
					}
				}
			}
		}

		internal void RemovePluginSetting( Guid pluginId, string key )
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "pluginID", pluginId );
					cmd.AddParameter( "key", key );

					cmd.CommandText = "DELETE FROM plugin_settings WHERE PluginID = @pluginID AND [Key] = @key;";
					cmd.Prepare();

					cmd.ExecuteNonQuery();
				}
			}
		}

		internal void Save()
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				foreach( Folder f in Folders )
				{
					f.Save( connection );
				}
			}
		}

		internal void SetDefaults()
		{
			Type type = typeof( SystemSetting );
			using( DbConnection connection = Factory.OpenConnection() )
			{
				DbTransaction transaction = connection.BeginTransaction();
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
			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "pluginID", pluginID );
					cmd.AddParameter( "key", key );
					cmd.AddParameter( "value", value );

					cmd.CommandText = "INSERT INTO plugin_settings (PluginID, [Key], [Value]) VALUES( @pluginID ,@key, @value );";
					cmd.Prepare();

					cmd.ExecuteNonQuery();
				}
			}
		}

		internal void SetValue( SystemSetting setting, object value )
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "key", setting.ToString() );
					cmd.AddParameter( "value", value );

					cmd.CommandText = "UPDATE settings SET [Value] = @value WHERE [Key] = @key;";
					cmd.Prepare();

					cmd.ExecuteNonQuery();
				}
			}
		}

		public ObservableCollection<Folder> Folders { get; private set; }

		private DbConnectionFactory Factory;
	}
}
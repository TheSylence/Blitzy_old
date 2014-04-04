// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.SQLite;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

		[DefaultValue( "Ctrl+Alt, Space" )]
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
		AutoCatalogRebuild
	}

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

		T Plugin.ISettings.GetSystemSetting<T>( SystemSetting setting )
		{
			return GetValue<T>( setting );
		}

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
				Type targetType = typeof( T );

				if( targetType == typeof( bool ) )
				{
					if( value.GetType() == typeof( int ) )
					{
						value = ( (int)value ) == 1;
					}
					else if( value.GetType() == typeof( string ) )
					{
						value = Convert.ToInt32( value.ToString() ) == 1;
					}
				}

				return (T)Convert.ChangeType( value, targetType );
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
						Folder f = new Folder();
						f.ID = reader.GetInt32( 0 );
						f.Load( Connection );

						Folders.Add( f );
					}
				}
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
			catch
			{
				transaction.Rollback();
				throw;
			}
		}

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
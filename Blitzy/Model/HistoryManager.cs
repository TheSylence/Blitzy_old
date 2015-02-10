using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	public class HistoryManager : BaseObject
	{
		internal HistoryManager( DbConnectionFactory factory, Settings settings )
		{
			Factory = factory;
			Settings = settings;
			Commands = new ObservableCollection<string>();

			using( DbConnection connection = Factory.OpenConnection() )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.CommandText = "SELECT Command FROM history ORDER BY HistoryID LIMIT 0, @historyCount";
					cmd.AddParameter( "historyCount", settings.GetValue<int>( SystemSetting.HistoryCount ) );
					cmd.Prepare();

					using( DbDataReader reader = cmd.ExecuteReader() )
					{
						while( reader.Read() )
						{
							Commands.Add( reader.GetString( 0 ) );
						}
					}
				}
			}
		}

		public void AddItem( string command )
		{
			if( command.Equals( Commands.LastOrDefault(), StringComparison.OrdinalIgnoreCase ) )
			{
				return;
			}

			Commands.Add( command );

			while( Commands.Count > Settings.GetValue<int>( SystemSetting.HistoryCount ) )
			{
				Commands.RemoveAt( 0 );
			}
		}

		public void Clear()
		{
			Commands.Clear();
		}

		public void Save()
		{
			using( DbConnection connection = Factory.OpenConnection() )
			{
				DbTransaction transaction = connection.BeginTransaction( IsolationLevel.ReadCommitted );
				try
				{
					using( DbCommand cmd = connection.CreateCommand() )
					{
						cmd.Transaction = transaction;
						cmd.CommandText = "DELETE FROM history";
						cmd.ExecuteNonQuery();
					}

					for( int i = 0; i < Math.Min( Settings.GetValue<int>( SystemSetting.HistoryCount ), Commands.Count ); ++i )
					{
						using( DbCommand cmd = connection.CreateCommand() )
						{
							cmd.Transaction = transaction;
							cmd.CommandText = "INSERT INTO history ( HistoryID, Command ) VALUES ( @id, @cmd );";
							cmd.AddParameter( "id", i );
							cmd.AddParameter( "cmd", Commands[i] );

							cmd.ExecuteNonQuery();
						}
					}

					transaction.Commit();
				}
				catch( Exception ex )
				{
					LogError( "Failed to save command history: {0}", ex );
					transaction.Rollback();
				}
			}
		}

		public ObservableCollection<string> Commands { get; internal set; }

		public string SelectedItem
		{
			get
			{
				return _SelectedItem;
			}

			set
			{
				if( _SelectedItem == value )
				{
					return;
				}

				RaisePropertyChanging( () => SelectedItem );
				_SelectedItem = value;
				RaisePropertyChanged( () => SelectedItem );
			}
		}

		private readonly Settings Settings;
		private string _SelectedItem;
		private DbConnectionFactory Factory;
	}
}
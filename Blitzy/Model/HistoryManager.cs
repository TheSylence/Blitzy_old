using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Data.SQLite;
using System.Linq;

namespace Blitzy.Model
{
	public class HistoryManager : BaseObject
	{
		#region Constructor

		internal HistoryManager( Settings settings )
		{
			Settings = settings;
			Commands = new ObservableCollection<string>();

			using( DbCommand cmd = settings.Connection.CreateCommand() )
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

		#endregion Constructor

		#region Methods

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
			DbTransaction transaction = Settings.Connection.BeginTransaction( IsolationLevel.ReadCommitted );
			try
			{
				using( DbCommand cmd = Settings.Connection.CreateCommand() )
				{
					cmd.Transaction = transaction;
					cmd.CommandText = "DELETE FROM history";
					cmd.ExecuteNonQuery();
				}

				for( int i = 0; i < Math.Min( Settings.GetValue<int>( SystemSetting.HistoryCount ), Commands.Count ); ++i )
				{
					using( DbCommand cmd = Settings.Connection.CreateCommand() )
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

		#endregion Methods

		#region Properties

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

		private string _SelectedItem;

		#endregion Properties

		#region Attributes

		private readonly Settings Settings;

		#endregion Attributes
	}
}
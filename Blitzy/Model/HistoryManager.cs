// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	public class HistoryManager : BaseObject
	{
		#region Constructor

		internal HistoryManager( Settings settings )
		{
			Settings = settings;
			Commands = new ObservableCollection<string>();

			using( SQLiteCommand cmd = settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT Command FROM history ORDER BY HistoryID LIMIT 0, @historyCount";

				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "historyCount";
				param.Value = settings.GetValue<int>( SystemSetting.HistoryCount );
				cmd.Parameters.Add( param );
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
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
			SQLiteTransaction transaction = Settings.Connection.BeginTransaction( System.Data.IsolationLevel.ReadCommitted );
			try
			{
				using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
				{
					cmd.Transaction = transaction;
					cmd.CommandText = "DELETE FROM history";
					cmd.ExecuteNonQuery();
				}

				for( int i = 0; i < Math.Min( Settings.GetValue<int>( SystemSetting.HistoryCount ), Commands.Count ); ++i )
				{
					using( SQLiteCommand cmd = Settings.Connection.CreateCommand() )
					{
						cmd.Transaction = transaction;
						cmd.CommandText = "INSERT INTO history ( HistoryID, Command ) VALUES ( @id, @cmd );";

						SQLiteParameter param = cmd.CreateParameter();
						param.ParameterName = "id";
						param.Value = i;
						cmd.Parameters.Add( param );

						param = cmd.CreateParameter();
						param.ParameterName = "cmd";
						param.Value = Commands[i];
						cmd.Parameters.Add( param );

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

		private string _SelectedItem;

		public ObservableCollection<string> Commands { get; private set; }

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

		#endregion Properties

		#region Attributes

		private Settings Settings;

		#endregion Attributes
	}
}
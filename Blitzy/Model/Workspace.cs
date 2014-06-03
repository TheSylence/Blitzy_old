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
	internal class Workspace : ModelBase
	{
		#region Constructor

		public Workspace()
		{
			Items = new ObservableCollection<WorkspaceItem>();
		}

		#endregion Constructor

		#region Methods

		public override void Delete( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "WorkspaceID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "DELETE FROM workspaces WHERE WorkspaceID = @WorkspaceID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();

				// TODO: Delete items that belong to this workspace
			}
		}

		public override void Load( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "WorkspaceID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Name FROM workspaces WHERE WorkspaceID = @WorkspaceID;";
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						throw new TypeLoadException( "Failed to read workspace from database" );
					}

					Name = reader.GetString( 0 );
				}
			}

			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "WorkspaceID";
				param.Value = ID;
				cmd.Parameters.Add( param );
				cmd.CommandText = "SELECT ItemID FROM workspace_items WHERE WorkspaceID = @WorkspaceID";
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						WorkspaceItem item = new WorkspaceItem();
						item.ItemID = reader.GetInt32( 0 );

						item.Load( connection );
						Items.Add( item );
					}
				}
			}

			ExistsInDatabase = true;
		}

		public override void Save( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "WorkspaceID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Name";
				param.Value = Name;
				cmd.Parameters.Add( param );

				if( ExistsInDatabase )
				{
					cmd.CommandText = "UPDATE workspaces SET Name = @Name WHERE WorkspaceID = @WorkspaceID";
				}
				else
				{
					cmd.CommandText = "INSERT INTO workspaces (WorkspaceID, Name) VALUES (@WorkspaceID, @Name);";
				}

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			if( ExistsInDatabase )
			{
				using( SQLiteCommand cmd = connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "WorkspaceID";
					param.Value = ID;
					cmd.Parameters.Add( param );

					cmd.CommandText = "DELETE FROM workspace_items WHERE WorkspaceID = @WorkspaceID";
					cmd.ExecuteNonQuery();
				}
			}

			foreach( WorkspaceItem item in Items )
			{
				item.ExistsInDatabase = false;
				item.Save( connection );
			}

			ExistsInDatabase = true;
		}

		#endregion Methods

		#region Properties

		private int _ID;

		private string _Name;

		public int ID
		{
			get
			{
				return _ID;
			}

			set
			{
				if( _ID == value )
				{
					return;
				}

				RaisePropertyChanging( () => ID );
				_ID = value;
				RaisePropertyChanged( () => ID );
			}
		}

		public ObservableCollection<WorkspaceItem> Items { get; private set; }

		public string Name
		{
			get
			{
				return _Name;
			}

			set
			{
				if( _Name == value )
				{
					return;
				}

				RaisePropertyChanging( () => Name );
				_Name = value;
				RaisePropertyChanged( () => Name );
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
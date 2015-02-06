using System;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Data.SQLite;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class Workspace : ModelBase
	{
		public Workspace()
		{
			Items = new ObservableCollection<WorkspaceItem>();
		}

		public override void Delete( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "WorkspaceID", ID );

				cmd.CommandText = "DELETE FROM workspaces WHERE WorkspaceID = @WorkspaceID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}

			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "WorkspaceID", ID );

				cmd.CommandText = "DELETE FROM workspace_items WHERE WorkspaceID = @WorkspaceID";
				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( DbConnection connection )
		{
			Items.Clear();

			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "WorkspaceID", ID );

				cmd.CommandText = "SELECT Name FROM workspaces WHERE WorkspaceID = @WorkspaceID;";
				cmd.Prepare();

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						throw new TypeLoadException( "Failed to read workspace from database" );
					}

					Name = reader.GetString( 0 );
				}
			}

			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "WorkspaceID", ID );
				cmd.CommandText = "SELECT ItemID FROM workspace_items WHERE WorkspaceID = @WorkspaceID ORDER BY ItemOrder";
				cmd.Prepare();

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						WorkspaceItem item = new WorkspaceItem { ItemID = reader.GetInt32( 0 ) };

						item.Load( connection );
						Items.Add( item );
					}
				}
			}

			ExistsInDatabase = true;
		}

		public override void Save( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "WorkspaceID", ID );
				cmd.AddParameter( "Name", Name );

				cmd.CommandText = ExistsInDatabase ?
					"UPDATE workspaces SET Name = @Name WHERE WorkspaceID = @WorkspaceID" :
					"INSERT INTO workspaces (WorkspaceID, Name) VALUES (@WorkspaceID, @Name);";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			if( ExistsInDatabase )
			{
				using( DbCommand cmd = connection.CreateCommand() )
				{
					cmd.AddParameter( "WorkspaceID", ID );

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

		private int _ID;

		private string _Name;
	}
}
using System;
using System.Data.Common;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class WorkspaceItem : ModelBase
	{
		public override void Delete( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "ItemID", ItemID );

				cmd.CommandText = "DELETE FROM workspace_items WHERE ItemID = @ItemID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "ItemID", ItemID );

				cmd.CommandText = "SELECT ItemOrder, WorkspaceID, ItemCommand FROM workspace_items WHERE ItemID = @ItemID;";
				cmd.Prepare();

				using( DbDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						throw new TypeLoadException( "Failed to read folder from database" );
					}

					ItemOrder = reader.GetInt32( 0 );
					WorkspaceID = reader.GetInt32( 1 );
					ItemCommand = reader.GetString( 2 );
				}
			}

			ExistsInDatabase = true;
		}

		public override void Save( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "ItemID", ItemID );
				cmd.AddParameter( "ItemCommand", ItemCommand );
				cmd.AddParameter( "ItemOrder", ItemOrder );
				cmd.AddParameter( "WorkspaceID", WorkspaceID );

				cmd.CommandText = ExistsInDatabase ?
					"UPDATE workspace_items SET ItemCommand = @ItemCommand, ItemOrder = @ItemOrder, WorkspaceID = @WorkspaceID WHERE ItemID = @ItemID" :
					"INSERT INTO workspace_items (ItemID, ItemCommand, ItemOrder, WorkspaceID) VALUES (@ItemID, @ItemCommand, @ItemOrder, @WorkspaceID);";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			ExistsInDatabase = true;
		}

		public string ItemCommand
		{
			get
			{
				return _ItemCommand;
			}

			set
			{
				if( _ItemCommand == value )
				{
					return;
				}

				_ItemCommand = value;
				RaisePropertyChanged( () => ItemCommand );
			}
		}

		public int ItemID
		{
			get
			{
				return _ItemID;
			}

			set
			{
				if( _ItemID == value )
				{
					return;
				}

				_ItemID = value;
				RaisePropertyChanged( () => ItemID );
			}
		}

		public int ItemOrder
		{
			get
			{
				return _ItemOrder;
			}

			set
			{
				if( _ItemOrder == value )
				{
					return;
				}

				_ItemOrder = value;
				RaisePropertyChanged( () => ItemOrder );
			}
		}

		public int WorkspaceID
		{
			get
			{
				return _WorkspaceID;
			}

			set
			{
				if( _WorkspaceID == value )
				{
					return;
				}

				_WorkspaceID = value;
				RaisePropertyChanged( () => WorkspaceID );
			}
		}

		private string _ItemCommand;
		private int _ItemID;
		private int _ItemOrder;
		private int _WorkspaceID;
	}
}
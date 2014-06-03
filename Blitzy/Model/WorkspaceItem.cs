// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model
{
	internal class WorkspaceItem : ModelBase
	{
		#region Constructor

		#endregion Constructor

		#region Methods

		public override void Delete( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "ItemID";
				param.Value = ItemID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "DELETE FROM workspace_items WHERE ItemID = @ItemID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "ItemID";
				param.Value = ItemID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT ItemOrder, WorkspaceID, ItemCommand FROM workspace_items WHERE ItemID = @ItemID;";
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
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

		public override void Save( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "ItemID";
				param.Value = ItemID;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "ItemCommand";
				param.Value = ItemCommand;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "ItemOrder";
				param.Value = ItemOrder;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "WorkspaceID";
				param.Value = WorkspaceID;
				cmd.Parameters.Add( param );

				if( ExistsInDatabase )
				{
					cmd.CommandText = "UPDATE workspace_items SET ItemCommand = @ItemCommand, ItemOrder = @ItemOrder, WorkspaceID = @WorkspaceID WHERE ItemID = @ItemID";
				}
				else
				{
					cmd.CommandText = "INSERT INTO workspace_items (ItemID, ItemCommand, ItemOrder, WorkspaceID) VALUES (@ItemID, @ItemCommand, @ItemOrder, @WorkspaceID);";
				}

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			ExistsInDatabase = true;
		}

		#endregion Methods

		#region Properties

		private string _ItemCommand;
		private int _ItemID;
		private int _ItemOrder;
		private int _WorkspaceID;

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

				RaisePropertyChanging( () => ItemCommand );
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

				RaisePropertyChanging( () => ItemID );
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

				RaisePropertyChanging( () => ItemOrder );
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

				RaisePropertyChanging( () => WorkspaceID );
				_WorkspaceID = value;
				RaisePropertyChanged( () => WorkspaceID );
			}
		}

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
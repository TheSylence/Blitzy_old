// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;

namespace Blitzy.ViewModel
{
	internal class WorkspaceSettingsViewModel : SettingsViewModelBase
	{
		#region Constructor

		public WorkspaceSettingsViewModel( SettingsViewModel baseVM )
			: base( baseVM )
		{
			Workspaces = new ObservableCollection<Workspace>();

			using( SQLiteCommand cmd = BaseVM.Settings.Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT WorkspaceID FROM workspaces";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Workspace workspace = new Workspace();
						workspace.ID = reader.GetInt32( 0 );

						workspace.Load( BaseVM.Settings.Connection );
						Workspaces.Add( workspace );
					}
				}
			}
		}

		#endregion Constructor

		#region Methods

		public override void Save()
		{
			foreach( Workspace ws in Workspaces )
			{
				ws.Save( Settings.Connection );
			}
		}

		#endregion Methods

		#region Properties

		public ObservableCollection<Workspace> Workspaces { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
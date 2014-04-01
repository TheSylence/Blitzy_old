// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	internal class Settings : ObservableObject
	{
		#region Constructor

		public Settings( SQLiteConnection connection )
		{
			Connection = connection;
			Folders = new ObservableCollection<Folder>();
		}

		#endregion Constructor

		#region Methods

		internal void Load()
		{
			using( SQLiteCommand cmd = Connection.CreateCommand() )
			{
				cmd.CommandText = "SELECT FolderID, Path, Recursive FROM folders";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Folder f = new Folder();
						f.ID = reader.GetInt32( 0 );
						f.Path = reader.GetString( 1 );
						f.IsRecursive = reader.GetInt32( 2 ) == 1;

						Folders.Add( f );
					}
				}
			}
		}

		internal void SetDefaults()
		{
			// TODO:
		}

		#endregion Methods

		#region Properties

		public ObservableCollection<Folder> Folders { get; private set; }

		#endregion Properties

		#region Attributes

		private SQLiteConnection Connection;

		#endregion Attributes
	}
}
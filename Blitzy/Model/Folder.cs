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
	internal class Folder : ObservableObject
	{
		#region Constructor

		public Folder()
		{
			Rules = new ObservableCollection<string>();
			Excludes = new ObservableCollection<string>();
		}

		#endregion Constructor

		#region Methods

		public void Load( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Rule FROM folder_rules WHERE FolderID = @folderID";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Rules.Add( reader.GetString( 0 ) );
					}
				}
			}

			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Exclude FROM folder_excludes WHERE FolderID = @folderID";

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Excludes.Add( reader.GetString( 0 ) );
					}
				}
			}
		}

		#endregion Methods

		#region Properties

		private bool _IsRecursive;

		private string _Path;

		public ObservableCollection<string> Excludes { get; private set; }

		public int ID { get; set; }

		public bool IsRecursive
		{
			get
			{
				return _IsRecursive;
			}

			set
			{
				if( _IsRecursive == value )
				{
					return;
				}

				RaisePropertyChanging( () => IsRecursive );
				_IsRecursive = value;
				RaisePropertyChanged( () => IsRecursive );
			}
		}

		public string Path
		{
			get
			{
				return _Path;
			}

			set
			{
				if( _Path == value )
				{
					return;
				}

				RaisePropertyChanging( () => Path );
				_Path = value;
				RaisePropertyChanged( () => Path );
			}
		}

		public ObservableCollection<string> Rules { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	internal class Folder : ModelBase
	{
		#region Constructor

		public Folder()
		{
			Rules = new ObservableCollection<string>();
			Excludes = new ObservableCollection<string>();
		}

		#endregion Constructor

		#region Methods

		public override void Delete( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "DELETE FROM folders WHERE FolderID = @folderID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();

				// TODO: Delete rules and exludes that belong to this folder
			}
		}

		public IEnumerable<string> GetFiles()
		{
			SearchOption options = IsRecursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;

			foreach( string rule in Rules )
			{
				string[] fileList = new string[0];

				try
				{
					fileList = Directory.GetFiles( Path, rule, options );
				}
				catch( Exception ex )
				{
					LogError( "While loading files: {0}", ex );
				}

				foreach( string file in fileList )
				{
					if( !Excludes.Any( e => IsExcluded( file, e ) ) )
					{
						yield return file;
					}
				}
			}
		}

		public override void Load( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Path, Recursive FROM folders WHERE FolderID = @folderID;";
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						throw new TypeLoadException( "Failed to read folder from database" );
					}

					Path = reader.GetString( 0 );
					IsRecursive = reader.GetInt32( 1 ) == 1;
				}
			}

			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Rule FROM folder_rules WHERE FolderID = @folderID";
				cmd.Prepare();

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
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					while( reader.Read() )
					{
						Excludes.Add( reader.GetString( 0 ) );
					}
				}
			}

			ExistsInDatabase = true;
		}

		public override void Save( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "folderID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Path";
				param.Value = Path;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Recursive";
				param.Value = IsRecursive ? 1 : 0;
				cmd.Parameters.Add( param );

				if( ExistsInDatabase )
				{
					cmd.CommandText = "UPDATE folders SET Path = @Path, Recursive = @Recursive WHERE FolderID = @folderID";
				}
				else
				{
					cmd.CommandText = "INSERT INTO folders (FolderID, Path, Recursive) VALUES (@folderID, @Path, @Recursive);";
				}

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			if( ExistsInDatabase )
			{
				using( SQLiteCommand cmd = connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "folderID";
					param.Value = ID;
					cmd.Parameters.Add( param );

					cmd.CommandText = "DELETE FROM folder_rules WHERE FolderID = @folderID";

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}

				using( SQLiteCommand cmd = connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "folderID";
					param.Value = ID;
					cmd.Parameters.Add( param );

					cmd.CommandText = "DELETE FROM folder_excludes WHERE FolderID = @folderID";

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}
			}

			foreach( string rule in Rules )
			{
				using( SQLiteCommand cmd = connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "folderID";
					param.Value = ID;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "rule";
					param.Value = rule;
					cmd.Parameters.Add( param );

					cmd.CommandText = "INSERT INTO folder_rules (FolderID, Rule) VALUES (@folderID, @rule);";

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}
			}

			foreach( string exclude in Excludes )
			{
				using( SQLiteCommand cmd = connection.CreateCommand() )
				{
					SQLiteParameter param = cmd.CreateParameter();
					param.ParameterName = "folderID";
					param.Value = ID;
					cmd.Parameters.Add( param );

					param = cmd.CreateParameter();
					param.ParameterName = "exclude";
					param.Value = exclude;
					cmd.Parameters.Add( param );

					cmd.CommandText = "INSERT INTO folder_excludes (FolderID, Exclude) VALUES (@folderID, @exclude);";

					cmd.Prepare();
					cmd.ExecuteNonQuery();
				}
			}

			ExistsInDatabase = true;
		}

		private static bool IsExcluded( string path, string exclude )
		{
			Regex ex = new Regex( exclude.WildcardToRegex() );
			path = System.IO.Path.GetFileName( path );
			return ex.IsMatch( path );
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
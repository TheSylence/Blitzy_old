// $Id$

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Blitzy.Utility;

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
			}

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

		public IEnumerable<string> GetFiles()
		{
			return GetFilesInFolder( Path );
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

				cmd.CommandText = ExistsInDatabase ?
					"UPDATE folders SET Path = @Path, Recursive = @Recursive WHERE FolderID = @folderID" :
					"INSERT INTO folders (FolderID, Path, Recursive) VALUES (@folderID, @Path, @Recursive);";

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
			string pattern = exclude.WildcardToRegex();
			Regex ex;
			if( !RegexCache.TryGetValue( pattern, out ex ) )
			{
				ex = new Regex( pattern );
				RegexCache.Add( pattern, ex );
			}

			path = System.IO.Path.GetFileName( path );
			return ex.IsMatch( path );
		}

		private IEnumerable<string> GetFilesInFolder( string folder )
		{
			IEnumerable<string> fileList = Enumerable.Empty<string>();
			DirectoryInfo topDirectory = new DirectoryInfo( folder );

			foreach( string rule in Rules )
			{
				string[] files = new string[0];

				try
				{
					files = Directory.GetFiles( folder, rule, SearchOption.TopDirectoryOnly );
				}
				catch
				{
				}

				fileList = fileList.Concat( files.Where( file => !Excludes.Any( e => IsExcluded( file, e ) ) ) );

				//IEnumerable<FileInfo> files = topDirectory.EnumerateFiles( rule, SearchOption.TopDirectoryOnly );
				//int filesLength = files.Count();
				//fileList = Enumerable.Range( 0, filesLength ).Select( i =>
				//{
				//	string filename = null;
				//	try
				//	{
				//		FileInfo inf = files.ElementAt( i );
				//		filename = inf.FullName;
				//	}
				//	catch
				//	{
				//	}

				//	return filename;
				//} ).Where( i => i != null && !Excludes.Any( e => IsExcluded( i, e ) ) );
			}

			if( IsRecursive )
			{
				string[] dirs = new string[0];

				try
				{
					dirs = Directory.GetDirectories( folder );
				}
				catch
				{
				}

				fileList = fileList.Concat( dirs.SelectMany( d => GetFilesInFolder( d ) ) );
				//IEnumerable<DirectoryInfo> dirs = topDirectory.EnumerateDirectories();
				//int dirsLength = dirs.Count();

				//IEnumerable<string> dirsList = Enumerable.Range( 0, dirsLength ).SelectMany( i =>
				//{
				//	try
				//	{
				//		DirectoryInfo inf = dirs.ElementAt( i );
				//		return GetFilesInFolder( inf.FullName );
				//	}
				//	catch
				//	{
				//	}

				//	return Enumerable.Empty<string>();
				//} );

				//fileList = Enumerable.Concat( fileList, dirsList );
			}

			return fileList;
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

		private static Dictionary<string, Regex> RegexCache = new Dictionary<string, Regex>();

		#endregion Attributes
	}
}
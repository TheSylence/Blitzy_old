// $Id$

using System;
using System.Data.SQLite;

namespace Blitzy.Model
{
	internal class WebyWebsite : ModelBase
	{
		#region Methods

		public override void Delete( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "webyID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "DELETE FROM weby_websites WHERE WebyID = @webyID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "webyID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Name, Description, URL, Icon FROM weby_websites WHERE WebyID = @webyID;";
				cmd.Prepare();

				using( SQLiteDataReader reader = cmd.ExecuteReader() )
				{
					if( !reader.Read() )
					{
						throw new TypeLoadException( "Failed to read folder from database" );
					}

					Name = reader.GetString( 0 );
					Description = reader.GetString( 1 );
					URL = reader.GetString( 2 );
					if( !reader.IsDBNull( 3 ) )
					{
						Icon = reader.GetString( 3 );
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
				param.ParameterName = "webyID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Name";
				param.Value = Name;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "Description";
				param.Value = Description;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "URL";
				param.Value = URL;
				cmd.Parameters.Add( param );

				param = cmd.CreateParameter();
				param.ParameterName = "icon";
				param.Value = Icon;
				cmd.Parameters.Add( param );

				cmd.CommandText = ExistsInDatabase ?
					"UPDATE weby_websites SET Name = @Name, Description = @Description, URL = @URL, Icon = @icon WHERE WebyID = @webyID" :
					"INSERT INTO weby_websites (WebyID, Name, Description, URL, Icon) VALUES (@webyID, @Name, @Description, @URL, @icon);";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			ExistsInDatabase = true;
		}

		#endregion Methods

		#region Properties

		private string _Description;
		private string _Icon;
		private int _ID;
		private string _Name;
		private string _URL;

		public string Description
		{
			get
			{
				return _Description;
			}

			set
			{
				if( _Description == value )
				{
					return;
				}

				RaisePropertyChanging( () => Description );
				_Description = value;
				RaisePropertyChanged( () => Description );
			}
		}

		public string Icon
		{
			get
			{
				return _Icon;
			}

			set
			{
				if( _Icon == value )
				{
					return;
				}

				RaisePropertyChanging( () => Icon );
				_Icon = value;
				RaisePropertyChanged( () => Icon );
			}
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

		public string URL
		{
			get
			{
				return _URL;
			}

			set
			{
				if( _URL == value )
				{
					return;
				}

				RaisePropertyChanging( () => URL );
				_URL = value;
				RaisePropertyChanged( () => URL );
			}
		}

		#endregion Properties
	}
}
using System;
using System.Data.Common;
using Blitzy.Plugin;

namespace Blitzy.Model
{
	internal class WebyWebsite : ModelBase
	{
		public override void Delete( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "webyID", ID );

				cmd.CommandText = "DELETE FROM weby_websites WHERE WebyID = @webyID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "webyID", ID );

				cmd.CommandText = "SELECT Name, Description, URL, Icon FROM weby_websites WHERE WebyID = @webyID;";
				cmd.Prepare();

				using( DbDataReader reader = cmd.ExecuteReader() )
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

		public override void Save( DbConnection connection )
		{
			using( DbCommand cmd = connection.CreateCommand() )
			{
				cmd.AddParameter( "webyID", ID );
				cmd.AddParameter( "Name", Name );
				cmd.AddParameter( "Description", Description );
				cmd.AddParameter( "URL", URL );
				cmd.AddParameter( "icon", Icon );

				cmd.CommandText = ExistsInDatabase ?
					"UPDATE weby_websites SET Name = @Name, Description = @Description, URL = @URL, Icon = @icon WHERE WebyID = @webyID" :
					"INSERT INTO weby_websites (WebyID, Name, Description, URL, Icon) VALUES (@webyID, @Name, @Description, @URL, @icon);";

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			ExistsInDatabase = true;
		}

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

				_URL = value;
				RaisePropertyChanged( () => URL );
			}
		}

		private string _Description;
		private string _Icon;
		private int _ID;
		private string _Name;
		private string _URL;
	}
}
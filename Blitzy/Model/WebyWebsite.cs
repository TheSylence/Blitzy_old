﻿// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

				cmd.CommandText = "DELETE FROM weby WHERE WebyID = @webyID;";
				cmd.Prepare();

				cmd.ExecuteNonQuery();
			}
		}

		public override void Load( System.Data.SQLite.SQLiteConnection connection )
		{
			using( SQLiteCommand cmd = connection.CreateCommand() )
			{
				SQLiteParameter param = cmd.CreateParameter();
				param.ParameterName = "webyID";
				param.Value = ID;
				cmd.Parameters.Add( param );

				cmd.CommandText = "SELECT Name, Description, URL FROM weby WHERE WebyID = @webyID;";
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
				}
			}

			ExistsInDatabase = true;
		}

		public override void Save( System.Data.SQLite.SQLiteConnection connection )
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

				if( ExistsInDatabase )
				{
					cmd.CommandText = "UPDATE weby SET Name = @Name, Description = @Description, URL = @URL WHERE WebyID = @webyID";
				}
				else
				{
					cmd.CommandText = "INSERT INTO weby (WebyID, Name, Description, URL) VALUES (@webyID, @Name, @Description, @URL);";
				}

				cmd.Prepare();
				cmd.ExecuteNonQuery();
			}

			ExistsInDatabase = true;
		}

		#endregion Methods

		#region Properties

		private string _Description;
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
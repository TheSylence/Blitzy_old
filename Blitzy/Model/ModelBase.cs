// $Id$

using System.Data.SQLite;

namespace Blitzy.Model
{
	internal abstract class ModelBase : BaseObject
	{
		#region Constructor

		#endregion Constructor

		#region Methods

		public abstract void Delete( SQLiteConnection connection );

		public abstract void Load( SQLiteConnection connection );

		public abstract void Save( SQLiteConnection connection );

		#endregion Methods

		#region Properties

		internal bool ExistsInDatabase;

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
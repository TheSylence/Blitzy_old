// $Id$

using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blitzy.Model
{
	internal abstract class ModelBase : LogObject
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
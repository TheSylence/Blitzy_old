using System.Data.Common;
using System.Data.SQLite;

namespace Blitzy.Model
{
	internal abstract class ModelBase : BaseObject
	{
		public abstract void Delete( DbConnection connection );

		public abstract void Load( DbConnection connection );

		public abstract void Save( DbConnection connection );

		internal bool ExistsInDatabase;
	}
}
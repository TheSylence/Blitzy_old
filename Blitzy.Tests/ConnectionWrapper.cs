using System;
using System.Data.Common;

namespace Blitzy.Tests
{
	internal class ConnectionWrapper : DbConnection
	{
		public ConnectionWrapper( DbConnection internalConnection )
		{
			InternalConnection = internalConnection;
		}

		public override void ChangeDatabase( string databaseName )
		{
			InternalConnection.ChangeDatabase( databaseName );
		}

		public override void Close()
		{
			throw new InvalidOperationException();
		}

		public override void Open()
		{
			InternalConnection.Open();
		}

		protected override DbTransaction BeginDbTransaction( System.Data.IsolationLevel isolationLevel )
		{
			return InternalConnection.BeginTransaction( isolationLevel );
		}

		protected override DbCommand CreateDbCommand()
		{
			return InternalConnection.CreateCommand();
		}

		public override string ConnectionString
		{
			get
			{
				return InternalConnection.ConnectionString;
			}
			set
			{
				InternalConnection.ConnectionString = value;
			}
		}

		public override string Database
		{
			get { return InternalConnection.Database; }
		}

		public override string DataSource
		{
			get { return InternalConnection.DataSource; }
		}

		public override string ServerVersion
		{
			get { return InternalConnection.ServerVersion; }
		}

		public override System.Data.ConnectionState State
		{
			get { return InternalConnection.State; }
		}

		private DbConnection InternalConnection;
	}
}
namespace Blitzy.Tests
{
	internal class TestConnectionFactory : DbConnectionFactory
	{
		internal override System.Data.Common.DbConnection OpenConnection()
		{
			return new ConnectionWrapper( TestHelper.Connection );
		}
	}
}
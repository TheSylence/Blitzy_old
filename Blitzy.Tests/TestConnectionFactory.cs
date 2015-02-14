namespace Blitzy.Tests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class TestConnectionFactory : DbConnectionFactory
	{
		internal override System.Data.Common.DbConnection OpenConnection()
		{
			return new ConnectionWrapper( TestHelper.Connection );
		}
	}
}
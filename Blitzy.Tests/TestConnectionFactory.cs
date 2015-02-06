using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
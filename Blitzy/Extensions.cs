using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy
{
	internal static class Extensions
	{
		internal static void AddParameter( this DbCommand command, string name, object value )
		{
			DbParameter parameter = command.CreateParameter();
			parameter.ParameterName = name;
			parameter.Value = value;

			command.Parameters.Add( parameter );
		}
	}
}
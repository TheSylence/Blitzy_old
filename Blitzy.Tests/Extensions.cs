// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Tests
{
	internal static class Extensions
	{
		internal static object GetDefaultValue( this Type type )
		{
			if( type.IsValueType )
			{
				return Activator.CreateInstance( type );
			}

			return null;
		}

		internal static object GetNonDefaultValue( this Type type )
		{
			Type[] numericTypes = new[]
			{
				typeof(ushort), typeof(short), typeof(uint),
				typeof(int), typeof(ulong), typeof(long),
				typeof(float), typeof(double), typeof(decimal)
			};

			if( numericTypes.Contains( type ) )
			{
				return Convert.ChangeType( 1, type );
			}
			else if( type == typeof( string ) )
			{
				return string.Empty;
			}
			else if( type == typeof( bool ) )
			{
				return true;
			}
			else if( !type.IsValueType )
			{
				return Activator.CreateInstance( type );
			}

			throw new NotSupportedException( type.ToString() );
		}
	}
}
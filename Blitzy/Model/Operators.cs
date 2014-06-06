// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Blitzy.Model
{
	internal interface IOperator
	{
		string Compute( List<string> args );

		int GetArgumentCount();
	}

	internal class AcosOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Acos( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class AddOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return ( double.Parse( args[1], CultureInfo.InvariantCulture ) + double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class AsinOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Asin( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class AtanOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Atan( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class CosOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Cos( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class DivOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return ( double.Parse( args[1], CultureInfo.InvariantCulture ) / double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class FacOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Fac( ulong.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}

		private ulong Fac( ulong n )
		{
			if( n == 0 )
			{
				return 1;
			}

			return n * Fac( n - 1 );
		}
	}

	internal class LnOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Log( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class LogOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Log( double.Parse( args[1], CultureInfo.InvariantCulture ), double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class MulOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return ( double.Parse( args[1], CultureInfo.InvariantCulture ) * double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class PowOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return ( Math.Pow( double.Parse( args[1], CultureInfo.InvariantCulture ), double.Parse( args[0], CultureInfo.InvariantCulture ) ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class RoundOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			double val = double.Parse( args[1], CultureInfo.InvariantCulture );
			int dec = int.Parse( args[0], CultureInfo.InvariantCulture );
			return Math.Round( val, dec ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class SinOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Sin( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class SqrtOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Sqrt( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}

	internal class SubOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return ( double.Parse( args[1], CultureInfo.InvariantCulture ) - double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 2;
		}
	}

	internal class TanOperator : IOperator
	{
		public string Compute( List<string> args )
		{
			return Math.Tan( double.Parse( args[0], CultureInfo.InvariantCulture ) ).ToString( CultureInfo.InvariantCulture );
		}

		public int GetArgumentCount()
		{
			return 1;
		}
	}
}
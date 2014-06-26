// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using btbapi;

namespace Blitzy.Tests
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal static class Extensions
	{
		public static Task<T> StartSTATask<T>( this Func<T> func )
		{
			var tcs = new TaskCompletionSource<T>();
			Thread thread = new Thread( () =>
			{
				if( Thread.CurrentThread.GetApartmentState() != ApartmentState.STA )
				{
					throw new InvalidOperationException( "STAThread is not in STA Apartment" );
				}

				try
				{
					tcs.SetResult( func() );
				}
				catch( Exception e )
				{
					tcs.SetException( e );
				}
			} );
			thread.SetApartmentState( ApartmentState.STA );
			thread.Start();
			return tcs.Task;
		}

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
			else if( type == typeof( VersionInfo ) )
			{
				return new VersionInfo( HttpStatusCode.OK, new Version( 1, 2 ), new Uri( "http://test.com" ), "123", 12344, new Dictionary<Version, string>() );
			}
			else if( type == typeof( ErrorReport ) )
			{
				return new ErrorReport( new Exception(), new System.Diagnostics.StackTrace( true ) );
			}
			else if( type == typeof( CultureInfo ) )
			{
				return CultureInfo.InvariantCulture;
			}
			else if( type.IsEnum )
			{
				return Enum.GetValues( type ).Cast<object>().Last();
			}
			else if( !type.IsValueType )
			{
				return Activator.CreateInstance( type );
			}
			else if( type == typeof( System.Windows.Input.Key ) )
			{
				return System.Windows.Input.Key.Escape;
			}
			else if( type == typeof( System.Windows.Input.ModifierKeys ) )
			{
				return System.Windows.Input.ModifierKeys.Control;
			}
			else if( type == typeof( DateTime ) )
			{
				return new DateTime( 2013, 3, 14 );
			}
			else if( type == typeof( TimeSpan ) )
			{
				return new TimeSpan( 2342342 );
			}

			throw new NotSupportedException( type.ToString() );
		}
	}
}
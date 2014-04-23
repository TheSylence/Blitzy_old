// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

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

			throw new NotSupportedException( type.ToString() );
		}
	}
}
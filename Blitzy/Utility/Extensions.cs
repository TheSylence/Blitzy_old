// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;

namespace Blitzy.Utility
{
	internal static class Extensions
	{
		public static double GetDiceCoefficent( this string str, string other, int n = 3 )
		{
			string[] strGrams = str.GetNGrams( n );
			string[] otherGrams = other.GetNGrams( n );

			int matches = strGrams.Intersect( otherGrams ).Count();

			return ( 2.0 * matches ) / (double)( strGrams.Length + otherGrams.Length );
		}

		public static string GetNameFromExpression<T>( this Expression<Func<T, object>> exp )
		{
			MemberExpression mem = exp.Body as MemberExpression;
			if( mem == null )
			{
				UnaryExpression un = exp.Body as UnaryExpression;
				if( un == null )
				{
					return null;
				}
				mem = un.Operand as MemberExpression;
			}

			if( mem == null )
			{
				return null;
			}
			return mem.Member.Name;
		}

		public static string Localize( this string str, string prefix = null, string suffix = null )
		{
			if( str == null )
			{
				throw new ArgumentNullException( "str" );
			}

			const string resRrefix = "Blitzy:Resources:";
			if( !str.StartsWith( resRrefix, StringComparison.OrdinalIgnoreCase ) )
			{
				str = resRrefix + str;
			}

			string value = LocalizeDictionary.Instance.GetLocalizedObject( str, null, Thread.CurrentThread.CurrentUICulture ) as string;

			if( prefix != null )
			{
				value = prefix + value;
			}
			if( suffix != null )
			{
				value += suffix;
			}

			return value;
		}

		public static string WildcardToRegex( this string pattern, bool wholeString = false )
		{
			string ex = Regex.Escape( pattern ).Replace( @"\*", ".*" ).Replace( @"\?", "." );

			if( wholeString )
			{
				return "^" + ex + "$";
			}

			return ex;
		}

		internal static DateTime LinkerTimestamp( this Assembly assembly )
		{
			string filePath = assembly.Location;
			const int c_PeHeaderOffset = 60;
			const int c_LinkerTimestampOffset = 8;
			byte[] b = new byte[2048];
			Stream s = null;

			try
			{
				s = new FileStream( filePath, FileMode.Open, FileAccess.Read );
				s.Read( b, 0, 2048 );
			}
			finally
			{
				if( s != null )
				{
					s.Close();
				}
			}

			int i = BitConverter.ToInt32( b, c_PeHeaderOffset );
			int secondsSince1970 = BitConverter.ToInt32( b, i + c_LinkerTimestampOffset );
			DateTime dt = new DateTime( 1970, 1, 1, 0, 0, 0 );
			dt = dt.AddSeconds( secondsSince1970 );
			dt = dt.AddHours( TimeZone.CurrentTimeZone.GetUtcOffset( dt ).Hours );
			return dt;
		}

		/// <summary>
		/// Gets the dice coefficent of this string with another string.
		/// </summary>
		/// <param name="str">This string.</param>
		/// <param name="other">The other string to compute the coefficent with.</param>
		/// <param name="n">The length of the N-Grams that is used</param>
		/// <returns>A factor between 0 and 1 that coresponds to the Dice-Coefficent </returns>
		private static string[] GetNGrams( this string str, int n )
		{
			string[] grams = new string[(int)Math.Ceiling( str.Length / (double)n )];
			int idx = 0;
			for( int i = 0; i < grams.Length; ++i )
			{
				if( idx + n > str.Length )
				{
					grams[i] = str.Substring( idx );
				}
				else
				{
					grams[i] = str.Substring( idx, n );
				}

				idx += n;
			}

			return grams;
		}
	}
}
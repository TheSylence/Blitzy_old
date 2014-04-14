// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;

namespace Blitzy
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
			UnaryExpression un = exp.Body as UnaryExpression;
			MemberExpression mem = un.Operand as MemberExpression;
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
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WPFLocalizeExtension.Engine;

namespace Blitzy
{
	internal static class Extensions
	{
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
	}
}
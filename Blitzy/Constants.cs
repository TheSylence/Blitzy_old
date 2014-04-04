// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy
{
	internal static class Constants
	{
		#region Constants

		internal const int APIVersion = 1;

#if DEBUG
		internal const string DataFileName = "data_debug.db";
#else
		internal const string DataFileName = "data.db";
#endif
		internal const string ErrorReportURL = "http://software.btbsoft.org/error.php?id=Blitzy";
		internal const string PluginsFolderName = "plugins";
		internal const string UpdateCheckURL = "http://software.btbsoft.org/check.php?id=Blitzy";

		#endregion Constants

		#region Properites

		[SuppressMessage( "Microsoft.Performance", "CA1811", Justification = "This is just plain bullshit" )]
		internal static string DataPath
		{
			get
			{
				string appdata = Environment.GetFolderPath( Environment.SpecialFolder.LocalApplicationData );
				string path = Path.Combine( appdata, "btbsoft", "Blitzy" );

				if( !Directory.Exists( path ) )
				{
					Directory.CreateDirectory( path );
				}

				return path;
			}
		}

		#endregion Properites
	}
}
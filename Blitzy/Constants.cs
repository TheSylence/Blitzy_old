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
		internal const string PluginsFolderName = "plugins";
		internal const string SoftwareName = "Blitzy";

		internal static string DataFileName
		{
			get
			{
				string fileName;

#if DEBUG
				fileName = "data_debug.db";
#else
				fileName = "data.db";
#endif

				if( RuntimeConfig.Tests )
				{
					fileName = "test_" + fileName;
				}

				return fileName;
			}
		}

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
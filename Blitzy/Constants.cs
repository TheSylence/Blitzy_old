// $Id$

using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace Blitzy
{
	internal static class Constants
	{
		#region Constants

		internal const int ApiVersion = 1;
		internal const string PluginsFolderName = "plugins";
		internal const string SoftwareName = "Blitzy";

		internal static string DataFileName
		{
			get
			{
#if DEBUG
				string fileName = "data_debug.db";
#else
				string fileName = "data.db";
#endif

				if( RuntimeConfig.Tests )
				{
					fileName = "test_" + fileName;
				}

				return fileName;
			}
		}

		internal static class CommandLine
		{
			internal static string InstallPlugin = "installplugin";
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
﻿using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;

namespace Blitzy
{
	internal static class Constants
	{
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

		internal static string PluginPath
		{
			get
			{
				return Path.Combine( Path.GetDirectoryName( Assembly.GetExecutingAssembly().Location ), PluginsFolderName );
			}
		}

		internal static class CommandLine
		{
			internal static string InstallPlugin = "installplugin";
		}

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
	}
}
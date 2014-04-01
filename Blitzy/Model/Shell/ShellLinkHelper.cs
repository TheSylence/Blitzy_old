// $Id$

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Model.Shell
{
	internal static class ShellLinkHelper
	{
		private static string windir, sys32, sysNative;

		private static string windirenv, sys32env, sysNativeenv;

		private static string x86, x64;

		public static string ResolveX64Path( string value )
		{
			if( Environment.Is64BitOperatingSystem )
			{
				if( !File.Exists( value ) )
				{
					// Dirty hack to work around a "bug" (ok let's call it unexpected behaviour in Windows):
					// The "Program Files" folder is ALWAYS resvoled to "Program Files (x86)" in a 32bit app
					// So we check if
					// - the path contains the "Program Files" folder
					// - and the file exists
					// if this is false we look for the 64bit folder
					if( x86 == null )
					{
						x86 = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFilesX86 );
					}

					if( x64 == null )
					{
						if( Environment.Is64BitProcess )
						{
							x64 = Environment.GetFolderPath( Environment.SpecialFolder.ProgramFiles );
						}
						else
						{
							x64 = Environment.ExpandEnvironmentVariables( "%ProgramW6432%" );
						}
					}

					if( value.StartsWith( x86, StringComparison.OrdinalIgnoreCase ) )
					{
						value = value.Replace( x86, x64 );
					}

					// And a second dirty hack: Wenn trying to access %windir%\system32 from a 32 bit app
					// you will be redirected to %windir%\syswow64
					// So we have to check
					// - if the resolved file contains "system32"
					//   - if this is true, check if the file exists in syswow64
					//     - if this is false, rewrite to use %windir%\Sysnative
					if( windirenv == null )
					{
						windirenv = Environment.GetFolderPath( Environment.SpecialFolder.Windows );
						sys32env = System.IO.Path.Combine( windirenv, "system32" );
						sysNativeenv = System.IO.Path.Combine( windirenv, "sysnative" );
					}

					if( value.StartsWith( sys32env, StringComparison.OrdinalIgnoreCase ) )
					{
						value = value.Replace( sys32env, sysNativeenv );
					}

					if( windir == null )
					{
						windir = "%windir%";
						sys32 = System.IO.Path.Combine( windir, "system32" );
						sysNative = System.IO.Path.Combine( windir, "sysnative" );
					}

					if( value.StartsWith( sys32, StringComparison.OrdinalIgnoreCase ) )
					{
						value = value.Replace( sys32, sysNative );
					}
				}
			}

			return value;
		}
	}
}
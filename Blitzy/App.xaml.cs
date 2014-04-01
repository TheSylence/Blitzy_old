﻿using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Threading;

namespace Blitzy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	public partial class App : Application
	{
		#region Constructor

		public App()
		{
			LogEnvironmentInfo();

			DispatcherHelper.Initialize();
			DialogServiceManager.RegisterServices();
		}

		#endregion Constructor

		#region Methods

		private void LogEnvironmentInfo()
		{
			LogHelper.LogInfo( GetType(), "Version {0}", Assembly.GetExecutingAssembly().GetName().Version );
			LogHelper.LogInfo( GetType(), "CLR: {0}", Environment.Version );
			LogHelper.LogInfo( GetType(), "{0} ({1})", Environment.OSVersion.ToString(), Environment.Is64BitOperatingSystem ? "x64" : "x86" );
			LogHelper.LogInfo( GetType(), "{0}bit process", Environment.Is64BitProcess ? 64 : 32 );
			using( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select Name, NumberOfLogicalProcessors, NumberOfCores from Win32_Processor" ) )
			{
				try
				{
					foreach( ManagementObject obj in searcher.Get() )
					{
						LogHelper.LogInfo( GetType(), "CPU: {0} ({1} physical, {2} logical)", obj["Name"], obj["NumberOfCores"], obj["NumberOfLogicalProcessors"] );
					}
				}
				catch
				{
					LogHelper.LogWarning( GetType(), "Failed to get CPU information" );
				}
			}

			using( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select Capacity from Win32_PhysicalMemory" ) )
			{
				try
				{
					ulong mem = 0;
					foreach( ManagementObject obj in searcher.Get() )
					{
						mem += Convert.ToUInt64( obj["Capacity"], CultureInfo.InvariantCulture );
					}

					LogHelper.LogInfo( GetType(), "RAM: {0}MB", mem / 1024.0 / 1024.0 );
				}
				catch
				{
					LogHelper.LogWarning( GetType(), "Failed to get RAM information" );
				}
			}

			LogHelper.LogInfo( GetType(), "Command: {0}", Environment.CommandLine );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
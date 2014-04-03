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
using System.Windows.Threading;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Threading;
using WPFLocalizeExtension.Engine;

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
			if( !SingleInstance.Start() )
			{
				SingleInstance.ShowFirstInstance();
				this.Shutdown( int.MinValue );
				return;
			}

			System.Threading.Thread.CurrentThread.Name = "Main";

			Exit += ( s, e ) => SingleInstance.Stop();

#if !DEBUG
			DispatcherUnhandledException += new DispatcherUnhandledExceptionEventArgs( Application_DispatcherUnhandledException );
#endif

			LocalizeDictionary.Instance.DefaultProvider.ProviderError += DefaultProvider_ProviderError;
			LocalizeDictionary.Instance.MissingKeyEvent += Instance_MissingKeyEvent;

			LogEnvironmentInfo();

			DispatcherHelper.Initialize();
			DialogServiceManager.RegisterServices();
		}

		private void Instance_MissingKeyEvent( object sender, MissingKeyEventArgs e )
		{
			LogHelper.LogDebug( MethodInfo.GetCurrentMethod().DeclaringType, "Missing resource key: {0}", e.Key );
		}

		#endregion Constructor

		#region Methods

		private void Application_DispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e )
		{
			try
			{
#if !DEBUG
				ExceptionDialog dlg = new ExceptionDialog( e.Exception );
				dlg.ShowDialog();

				e.Handled = true;
#endif
			}
			catch
			{
				// There already was an exception so recovering here is impossible
			}
			finally
			{
				SingleInstance.Stop();
#if !DEBUG
				System.Environment.Exit( -1 );
#endif
			}
		}

		private void DefaultProvider_ProviderError( object sender, WPFLocalizeExtension.Providers.ProviderErrorEventArgs args )
		{
			LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "{0} for key {1} on {2}", args.Message, args.Key, args.Object );
		}

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
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Management;
using System.Reflection;
using System.Windows;
using System.Windows.Threading;
using Blitzy.Utility;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using WPFLocalizeExtension.Engine;

namespace Blitzy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class App : Application
	{
		#region Constructor

		public App()
		{
			INativeMethods.Instance = new NativeMethods();

			if( !SingleInstance.Start() )
			{
				SingleInstance.ShowFirstInstance();
				this.Shutdown( int.MinValue );
				return;
			}

			System.Threading.Thread.CurrentThread.Name = "Main";

#if !DEBUG
			DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler( Application_DispatcherUnhandledException );
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

		protected override void OnExit( ExitEventArgs e )
		{
			ViewModelLocator.Cleanup();

			NotifyIcon.Dispose();
			SingleInstance.Stop();
			base.OnExit( e );
		}

		protected override void OnStartup( StartupEventArgs e )
		{
			base.OnStartup( e );

			NotifyIcon = (TaskbarIcon)FindResource( "NotifyIcon" );
		}

		private void Application_DispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e )
		{
			try
			{
#if !DEBUG
				Blitzy.View.Dialogs.ExceptionDialog dlg = new Blitzy.View.Dialogs.ExceptionDialog( e.Exception );
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
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Version {0}", Assembly.GetExecutingAssembly().GetName().Version );
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "CLR: {0}", Environment.Version );
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "{0} ({1})", Environment.OSVersion.ToString(), Environment.Is64BitOperatingSystem ? "x64" : "x86" );
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "{0}bit process", Environment.Is64BitProcess ? 64 : 32 );
			using( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select Name, NumberOfLogicalProcessors, NumberOfCores from Win32_Processor" ) )
			{
				try
				{
					foreach( ManagementObject obj in searcher.Get() )
					{
						LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "CPU: {0} ({1} physical, {2} logical)", obj["Name"], obj["NumberOfCores"], obj["NumberOfLogicalProcessors"] );
					}
				}
				catch
				{
					LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "Failed to get CPU information" );
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

					LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "RAM: {0}MB", mem / 1024.0 / 1024.0 );
				}
				catch
				{
					LogHelper.LogWarning( MethodInfo.GetCurrentMethod().DeclaringType, "Failed to get RAM information" );
				}
			}

			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Command: {0}", Environment.CommandLine );
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		private TaskbarIcon NotifyIcon;

		#endregion Attributes
	}
}
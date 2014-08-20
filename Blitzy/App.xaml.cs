using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Management;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using Blitzy.Messages;
using Blitzy.Utility;
using Blitzy.View.Dialogs;
using Blitzy.ViewModel;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Hardcodet.Wpf.TaskbarNotification;
using WPFLocalizeExtension.Engine;
using WPFLocalizeExtension.Providers;

namespace Blitzy
{
	/// <summary>
	/// Interaction logic for App.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class App
	{
		#region Constructor

		public App()
		{
			INativeMethods.Instance = new NativeMethods();

			string[] args = Environment.GetCommandLineArgs();
			if( args.Length > 0 )
			{
				if( args[0].Equals( Constants.CommandLine.InstallPlugin, StringComparison.OrdinalIgnoreCase ) )
				{
					ViewModelLocator vmloc = (ViewModelLocator)Application.Current.FindResource( "Locator" );
					Debug.Assert( vmloc != null );
					MainViewModel vm = vmloc.Main;

					vm.Plugins.InstallPlugin( args[1] );
					Shutdown( 0 );
					return;
				}
			}

			if( !SingleInstance.Start() )
			{
				SingleInstance.ShowFirstInstance();
				Shutdown( int.MinValue );
				return;
			}

			Thread.CurrentThread.Name = "Main";

			//#if !DEBUG
			DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler( Application_DispatcherUnhandledException );
			//#endif

			LocalizeDictionary.Instance.DefaultProvider.ProviderError += DefaultProvider_ProviderError;
			LocalizeDictionary.Instance.MissingKeyEvent += Instance_MissingKeyEvent;

			LogEnvironmentInfo();

			DispatcherHelper.Initialize();
			DialogServiceManager.RegisterServices();
			Messenger.Default.Register<LanguageMessage>( this, ChangeLanguage );
		}

		private void Instance_MissingKeyEvent( object sender, MissingKeyEventArgs e )
		{
			LogHelper.LogDebug( MethodBase.GetCurrentMethod().DeclaringType, "Missing resource key: {0}", e.Key );
		}

		#endregion Constructor

		#region Methods

		protected override void OnExit( ExitEventArgs e )
		{
			STAThread.Stop();
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

		private static void LogEnvironmentInfo()
		{
			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "Version {0}", Assembly.GetExecutingAssembly().GetName().Version );
			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "CLR: {0}", Environment.Version );
			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "{0} ({1})", Environment.OSVersion.ToString(), Environment.Is64BitOperatingSystem ? "x64" : "x86" );
			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "{0}bit process", Environment.Is64BitProcess ? 64 : 32 );
			using( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select Name, NumberOfLogicalProcessors, NumberOfCores from Win32_Processor" ) )
			{
				try
				{
					foreach( ManagementObject obj in searcher.Get() )
					{
						LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "CPU: {0} ({1} physical, {2} logical)", obj["Name"], obj["NumberOfCores"], obj["NumberOfLogicalProcessors"] );
					}
				}
				catch
				{
					LogHelper.LogWarning( MethodBase.GetCurrentMethod().DeclaringType, "Failed to get CPU information" );
				}
			}

			using( ManagementObjectSearcher searcher = new ManagementObjectSearcher( "select Capacity from Win32_PhysicalMemory" ) )
			{
				try
				{
					ulong mem = searcher.Get().Cast<ManagementObject>().Aggregate<ManagementObject, ulong>( 0, ( current, obj ) => current + Convert.ToUInt64( obj["Capacity"], CultureInfo.InvariantCulture ) );

					LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "RAM: {0}MB", mem / 1024.0 / 1024.0 );
				}
				catch
				{
					LogHelper.LogWarning( MethodBase.GetCurrentMethod().DeclaringType, "Failed to get RAM information" );
				}
			}

			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "Command: {0}", Environment.CommandLine );
		}

		private void Application_DispatcherUnhandledException( object sender, DispatcherUnhandledExceptionEventArgs e )
		{
			LogHelper.LogFatal( this, "Unhandled Exception: {0}", e.Exception );

			if( Debugger.IsAttached )
			{
				Debugger.Break();
			}

			try
			{
				ExceptionDialog dlg = new ExceptionDialog( e.Exception, new StackTrace( true ) );
				dlg.ShowDialog();

				e.Handled = true;
			}
			catch
			{
				// There already was an exception so recovering here is impossible
			}
			finally
			{
				SingleInstance.Stop();
				Environment.Exit( -1 );
			}
		}

		private void ChangeLanguage( LanguageMessage msg )
		{
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Changing language to {0}", msg.Language );

			LogHelper.LogDebug( MethodInfo.GetCurrentMethod().DeclaringType, "Current UI: {0}, Current Culture: {0}", Thread.CurrentThread.CurrentUICulture, Thread.CurrentThread.CurrentCulture );
			LocalizeDictionary.Instance.SetCurrentThreadCulture = true;
			LocalizeDictionary.Instance.Culture = msg.Language;
			LogHelper.LogDebug( MethodInfo.GetCurrentMethod().DeclaringType, "Current UI: {0}, Current Culture: {0}", Thread.CurrentThread.CurrentUICulture, Thread.CurrentThread.CurrentCulture );
		}

		private void DefaultProvider_ProviderError( object sender, ProviderErrorEventArgs args )
		{
			LogHelper.LogWarning( MethodBase.GetCurrentMethod().DeclaringType, "{0} for key {1} on {2}", args.Message, args.Key, args.Object );
		}

		#endregion Methods

		#region Attributes

		private TaskbarIcon NotifyIcon;

		#endregion Attributes
	}
}
/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Blitzy"
                           x:Key="Locator" />
  </Application.Resources>

  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using System;
using System.Collections.Generic;
using System.Reflection;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Practices.ServiceLocation;

namespace Blitzy.ViewModel
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	internal class ViewModelLocator
	{
		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider( () => SimpleIoc.Default );

			Register<MainViewModel>();

			Register<SettingsViewModel>();
			Register<HistoryViewModel>();

			Register<TextInputDialogViewModel>();
			Register<WebyWebsiteDialogViewModel>();
			Register<ChangelogDialogViewModel>();
			Register<DownloadDialogViewModel>();
			Register<PluginsDialogViewModel>();

			Register<NotifyIconViewModel>();

			SimpleIoc.Default.Register<DbConnectionFactory>();
			SimpleIoc.Default.Register<ViewServiceManager>( () => ViewServiceManager.Default );
			SimpleIoc.Default.Register<IMessenger>( () => Messenger.Default );
		}

		public static void Cleanup()
		{
			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "Cleaning up ViewModels..." );

			foreach( Type type in ViewModelTypes )
			{
				try
				{
					foreach( ViewModelBaseEx vm in ServiceLocator.Current.GetAllInstances( type ) )
					{
						vm.Cleanup();
						vm.Dispose();
					}
				}
				catch( Exception ex )
				{
					LogHelper.LogError( MethodInfo.GetCurrentMethod().DeclaringType, "While cleaning up {0}: {1}", type, ex );
				}
			}

			LogHelper.LogInfo( MethodBase.GetCurrentMethod().DeclaringType, "All ViewModels cleared" );
		}

		private void Register<T>() where T : class
		{
			SimpleIoc.Default.Register<T>();
			ViewModelTypes.Add( typeof( T ) );
		}

		public ChangelogDialogViewModel ChangelogDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<ChangelogDialogViewModel>();
			}
		}

		public DownloadDialogViewModel DownloadDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<DownloadDialogViewModel>();
			}
		}

		public HistoryViewModel History
		{
			get
			{
				return ServiceLocator.Current.GetInstance<HistoryViewModel>();
			}
		}

		public MainViewModel Main
		{
			get
			{
				return ServiceLocator.Current.GetInstance<MainViewModel>();
			}
		}

		public NotifyIconViewModel NotifyIcon
		{
			get
			{
				return ServiceLocator.Current.GetInstance<NotifyIconViewModel>();
			}
		}

		public PluginsDialogViewModel PluginsDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<PluginsDialogViewModel>();
			}
		}

		public SettingsViewModel Settings
		{
			get
			{
				return ServiceLocator.Current.GetInstance<SettingsViewModel>();
			}
		}

		public TextInputDialogViewModel TextInputDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<TextInputDialogViewModel>();
			}
		}

		public WebyWebsiteDialogViewModel WebyWebsiteDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<WebyWebsiteDialogViewModel>();
			}
		}

		private static readonly List<Type> ViewModelTypes = new List<Type>();
	}
}
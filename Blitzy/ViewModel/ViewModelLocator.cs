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
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Blitzy.ViewModel
{
	/// <summary>
	/// This class contains static references to all the view models in the
	/// application and provides an entry point for the bindings.
	/// </summary>
	internal class ViewModelLocator
	{
		#region Constructor

		private static List<Type> ViewModelTypes = new List<Type>();

		/// <summary>
		/// Initializes a new instance of the ViewModelLocator class.
		/// </summary>
		public ViewModelLocator()
		{
			ServiceLocator.SetLocatorProvider( () => SimpleIoc.Default );

			////if (ViewModelBase.IsInDesignModeStatic)
			////{
			////    // Create design time view services and models
			////    SimpleIoc.Default.Register<IDataService, DesignDataService>();
			////}
			////else
			////{
			////    // Create run time view services and models
			////    SimpleIoc.Default.Register<IDataService, DataService>();
			////}

			Register<MainViewModel>();

			Register<SettingsViewModel>();
			Register<HistoryViewModel>();

			Register<TextInputDialogViewModel>();
			Register<WebyWebsiteDialogViewModel>();
			Register<ChangelogDialogViewModel>();

			Register<NotifyIconViewModel>();
		}

		private void Register<T>() where T : class
		{
			SimpleIoc.Default.Register<T>();
			ViewModelTypes.Add( typeof( T ) );
		}

		#endregion Constructor

		#region ViewModels

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

		public SettingsViewModel Settings
		{
			get
			{
				return ServiceLocator.Current.GetInstance<SettingsViewModel>();
			}
		}

		#endregion ViewModels

		#region Dialogs

		public ChangelogDialogViewModel ChangelogDialog
		{
			get
			{
				return ServiceLocator.Current.GetInstance<ChangelogDialogViewModel>();
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

		#endregion Dialogs

		public static void Cleanup()
		{
			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "Cleaning up ViewModels..." );

			foreach( Type type in ViewModelTypes )
			{
				foreach( ViewModelBaseEx vm in ServiceLocator.Current.GetAllInstances( type ) )
				{
					vm.Cleanup();
					vm.Dispose();
				}
			}

			LogHelper.LogInfo( MethodInfo.GetCurrentMethod().DeclaringType, "All ViewModels cleared" );
		}
	}
}
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

			SimpleIoc.Default.Register<MainViewModel>();
			SimpleIoc.Default.Register<SettingsViewModel>();

			SimpleIoc.Default.Register<TextInputDialogViewModel>();
			SimpleIoc.Default.Register<WebyWebsiteDialogViewModel>();
		}

		#endregion Constructor

		#region ViewModels

		public MainViewModel Main
		{
			get
			{
				return ServiceLocator.Current.GetInstance<MainViewModel>();
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
			// TODO Clear the ViewModels
		}
	}
}
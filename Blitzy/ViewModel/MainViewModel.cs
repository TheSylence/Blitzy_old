using Blitzy.Model;
using Blitzy.ViewServices;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel
{
	public class MainViewModel : ViewModelBaseEx
	{
		#region Constructor

		/// <summary>
		/// Initializes a new instance of the MainViewModel class.
		/// </summary>
		public MainViewModel()
		{
			Database = ToDispose( new Database() );
			Settings = new Settings( Database.Connection );

			if( !Database.CheckExistance() )
			{
				Settings.SetDefaults();
			}
			else
			{
				Settings.Load();
			}

			////if (IsInDesignMode)
			////{
			////    // Code runs in Blend --> create design time data.
			////}
			////else
			////{
			////    // Code runs "for real"
			////}
		}

		#endregion Constructor

		#region Methods

		#endregion Methods

		#region Commands

		private RelayCommand _SettingsCommand;

		public RelayCommand SettingsCommand
		{
			get
			{
				return _SettingsCommand ??
					( _SettingsCommand = new RelayCommand( ExecuteSettingsCommand, CanExecuteSettingsCommand ) );
			}
		}

		private bool CanExecuteSettingsCommand()
		{
			return true;
		}

		private void ExecuteSettingsCommand()
		{
			DialogServiceManager.Show<SettingsService>( Settings );
		}

		#endregion Commands

		#region Properties

		internal Database Database { get; private set; }

		internal Settings Settings { get; private set; }

		#endregion Properties

		#region Attributes

		#endregion Attributes
	}
}
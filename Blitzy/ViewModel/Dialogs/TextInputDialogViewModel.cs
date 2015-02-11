using GalaSoft.MvvmLight.CommandWpf;

namespace Blitzy.ViewModel.Dialogs
{
	public class TextInputDialogViewModel : ViewModelBaseEx
	{
		public TextInputDialogViewModel()
			: base( null )
		{
		}

		private bool CanExecuteCancelCommand()
		{
			return true;
		}

		private bool CanExecuteOkCommand()
		{
			return true;
		}

		private void ExecuteCancelCommand()
		{
			Close( false );
		}

		private void ExecuteOkCommand()
		{
			Close( true );
		}

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
			}
		}

		public string Caption
		{
			get
			{
				return _Caption;
			}

			set
			{
				if( _Caption == value )
				{
					return;
				}

				_Caption = value;
				RaisePropertyChanged( () => Caption );
			}
		}

		public string Input
		{
			get
			{
				return _Input;
			}

			set
			{
				if( _Input == value )
				{
					return;
				}

				_Input = value;
				RaisePropertyChanged( () => Input );
			}
		}

		public string LabelText
		{
			get
			{
				return _LabelText;
			}

			set
			{
				if( _LabelText == value )
				{
					return;
				}

				_LabelText = value;
				RaisePropertyChanged( () => LabelText );
			}
		}

		public RelayCommand OkCommand
		{
			get
			{
				return _OkCommand ??
					( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );
			}
		}

		private RelayCommand _CancelCommand;
		private string _Caption;
		private string _Input;
		private string _LabelText;
		private RelayCommand _OkCommand;
	}
}
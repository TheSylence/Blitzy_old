// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	public class TextInputDialogViewModel : ViewModelBaseEx
	{
		#region Properties

		private string _Caption;
		private string _Input;
		private string _LabelText;

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

				RaisePropertyChanging( () => Caption );
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

				RaisePropertyChanging( () => Input );
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

				RaisePropertyChanging( () => LabelText );
				_LabelText = value;
				RaisePropertyChanged( () => LabelText );
			}
		}

		#endregion Properties

		#region Commands

		private RelayCommand _OkCommand;

		public RelayCommand OkCommand
		{
			get
			{
				return _OkCommand ??
					( _OkCommand = new RelayCommand( ExecuteOkCommand, CanExecuteOkCommand ) );
			}
		}

		private bool CanExecuteOkCommand()
		{
			return true;
		}

		private void ExecuteOkCommand()
		{
			Close( true );
		}

		#endregion Commands
	}
}
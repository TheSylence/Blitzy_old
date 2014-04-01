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
		#region Constructor

		public TextInputDialogViewModel()
		{
			OkCommand = new RelayCommand( () => Ok(), () => !string.IsNullOrWhiteSpace( Input ) );
		}

		#endregion Constructor

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
				_Caption = value;
				RaisePropertyChanged( "Caption" );
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
				_Input = value;
				RaisePropertyChanged( "Input" );
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
				_LabelText = value;
				RaisePropertyChanged( "LabelText" );
			}
		}

		#endregion Properties

		#region Commands

		public RelayCommand OkCommand { get; set; }

		private void Ok()
		{
			Close( true );
		}

		#endregion Commands
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	internal abstract class DialogViewModelBase<TModel> : ViewModelBaseEx, IRequestCloseViewModel
		where TModel : ModelBase
	{
		#region Constructor

		public DialogViewModelBase()
		{
		}

		#endregion Constructor

		#region Methods

		public override void Reset()
		{
			base.Reset();
			Model = (TModel)Activator.CreateInstance( typeof( TModel ) );
		}

		#endregion Methods

		#region Properties

		private TModel _Model;
		private bool _New;
		private string _Title;

		public TModel Model
		{
			get
			{
				return _Model;
			}

			set
			{
				if( _Model == value )
				{
					return;
				}

				RaisePropertyChanging( () => Model );
				_Model = value;
				RaisePropertyChanged( () => Model );
			}
		}

		public bool New
		{
			get
			{
				return _New;
			}

			set
			{
				if( _New == value )
				{
					return;
				}

				RaisePropertyChanging( () => New );
				_New = value;
				RaisePropertyChanged( () => New );

				string key;
				if( _New )
				{
					key = "Add";
				}
				else
				{
					key = "Edit";
				}

				Title = key.Localize();
			}
		}

		public string Title
		{
			get
			{
				return _Title;
			}

			set
			{
				if( _Title == value )
				{
					return;
				}

				RaisePropertyChanging( () => Title );
				_Title = value;
				RaisePropertyChanged( () => Title );
			}
		}

		#endregion Properties

		#region Commands

		private RelayCommand _CancelCommand;
		private RelayCommand _OkCommand;

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
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

		protected virtual bool CanExecuteOkCommand()
		{
			return true;
		}

		private bool CanExecuteCancelCommand()
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

		#endregion Commands
	}
}
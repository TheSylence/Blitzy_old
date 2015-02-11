using System;
using Blitzy.Model;
using Blitzy.Utility;
using GalaSoft.MvvmLight.Command;

namespace Blitzy.ViewModel.Dialogs
{
	internal abstract class DialogViewModelBase<TModel> : ViewModelBaseEx
		where TModel : ModelBase
	{
		public DialogViewModelBase( DbConnectionFactory factory = null )
			: base( factory )
		{
		}

		public override void Reset()
		{
			base.Reset();
			if( Model != null )
			{
				DisposeObject( Model );
			}

			Model = ToDispose( (TModel)Activator.CreateInstance( typeof( TModel ) ) );
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

		public RelayCommand CancelCommand
		{
			get
			{
				return _CancelCommand ??
					( _CancelCommand = new RelayCommand( ExecuteCancelCommand, CanExecuteCancelCommand ) );
			}
		}

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

				string key = _New ?
					"Add" :
					"Edit";

				Title = key.Localize();
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

		private RelayCommand _CancelCommand;
		private TModel _Model;
		private bool _New;
		private RelayCommand _OkCommand;
		private string _Title;
	}
}
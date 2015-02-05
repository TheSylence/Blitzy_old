

using Blitzy.Messages;
using Blitzy.Model;

namespace Blitzy.ViewModel
{
	internal class HistoryViewModel : ViewModelBaseEx
	{
		#region Constructor

		public HistoryViewModel()
		{
			MessengerInstance.Register<HistoryMessage>( this, OnMessage );
		}

		#endregion Constructor

		#region Methods

		private void OnMessage( HistoryMessage msg )
		{
			switch( msg.Type )
			{
				case HistoryMessageType.Show:
					Manager = msg.History;
					break;

				case HistoryMessageType.Down:
					++SelectedIndex;
					if( SelectedIndex >= Manager.Commands.Count )
					{
						SelectedIndex = 0;
					}
					break;

				case HistoryMessageType.Up:
					--SelectedIndex;
					if( SelectedIndex < 0 )
					{
						SelectedIndex = Manager.Commands.Count - 1;
					}
					break;

				default:
					return;
			}

			Manager.SelectedItem = Manager.Commands[SelectedIndex];
		}

		#endregion Methods

		#region Properties

		private HistoryManager _Manager;

		public HistoryManager Manager
		{
			get
			{
				return _Manager;
			}

			set
			{
				if( _Manager == value )
				{
					return;
				}

				RaisePropertyChanging( () => Manager );
				_Manager = value;
				RaisePropertyChanged( () => Manager );
			}
		}

		#endregion Properties

		#region Attributes

		private int SelectedIndex;

		#endregion Attributes
	}
}
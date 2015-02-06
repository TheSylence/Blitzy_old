using Blitzy.Messages;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.ViewModel
{
	internal class HistoryViewModel : ViewModelBaseEx
	{
		public HistoryViewModel( IMessenger messenger = null )
			: base( null, null, messenger )
		{
			MessengerInstance.Register<HistoryMessage>( this, OnMessage );
		}

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

		private HistoryManager _Manager;
		private int SelectedIndex;
	}
}
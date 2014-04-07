using System;

namespace Blitzy.ViewModel
{
	internal interface IRequestCloseViewModel
	{
		event EventHandler<CloseViewEventArgs> RequestClose;

		event EventHandler<EventArgs> RequestHide;
	}
}
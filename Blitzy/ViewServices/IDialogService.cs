// $Id$

using System.Windows;

namespace Blitzy.ViewServices
{
	public interface IDialogService
	{
		object Show( Window parent, object parameter = null );
	}
}
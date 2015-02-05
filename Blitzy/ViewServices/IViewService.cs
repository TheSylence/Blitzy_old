// $Id$

using System.Windows;

namespace Blitzy.ViewServices
{
	public interface IViewService
	{
		object Show( Window parent, object parameter = null );
	}
}
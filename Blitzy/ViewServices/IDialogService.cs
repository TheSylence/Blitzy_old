// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	public interface IDialogService
	{
		object Show( Window parent, object parameter = null );
	}
}
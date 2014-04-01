// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	internal class MessageBoxService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			MessageBoxParameter info = parameter as MessageBoxParameter;
			if( info == null )
			{
				throw new ArgumentException( "MessageBoxService braucht eine MessageBoxParameter als Parameter" );
			}

			if( parent == null )
			{
				return MessageBox.Show( info.Text, info.Caption, info.Button, info.Icon );
			}

			return MessageBox.Show( parent, info.Text, info.Caption, info.Button, info.Icon );
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	public class MessageBoxService : IDialogService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			MessageBoxParameter info = parameter as MessageBoxParameter;
			if( info == null )
			{
				throw new ArgumentException( "MessageBoxService needs a MessageBoxParameter as parameter" );
			}

			if( parent == null )
			{
				return MessageBox.Show( info.Text, info.Caption, info.Button, info.Icon );
			}

			return MessageBox.Show( parent, info.Text, info.Caption, info.Button, info.Icon );
		}
	}
}
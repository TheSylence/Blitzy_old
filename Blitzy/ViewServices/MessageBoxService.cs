using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	public class MessageBoxService : IViewService
	{
		public object Show( Window parent, object parameter = null )
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
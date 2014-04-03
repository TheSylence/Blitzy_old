// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	internal class MessageBoxServiceMock : IDialogService
	{
		internal MessageBoxResult Result;

		public MessageBoxServiceMock( MessageBoxResult result )
		{
			Result = result;
		}

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Result;
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	internal class DelegateServiceMock : IDialogService
	{
		public Func<object, object> Action;

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Action( parameter );
		}
	}
}
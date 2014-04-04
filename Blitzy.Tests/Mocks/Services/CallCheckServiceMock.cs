// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	internal class CallCheckServiceMock : IDialogService
	{
		public object Parameter;
		public bool WasCalled;

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			WasCalled = true;
			Parameter = parameter;

			return null;
		}
	}
}
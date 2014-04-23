// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class TextInputServiceMock : IDialogService
	{
		public string Text;

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Text;
		}
	}
}
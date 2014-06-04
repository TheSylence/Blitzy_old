// $Id$

using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class StringServiceMock : IDialogService
	{
		public string Value;

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Value;
		}
	}
}
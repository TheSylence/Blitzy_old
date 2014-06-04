// $Id$

using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
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
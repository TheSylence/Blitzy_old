using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class TextInputServiceMock : IViewService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Value;
		}

		public string Value;
	}
}
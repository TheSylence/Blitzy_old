using System.Windows;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MessageBoxServiceMock : IViewService
	{
		public MessageBoxServiceMock( MessageBoxResult result )
		{
			Result = result;
		}

		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Result;
		}

		internal MessageBoxResult Result;
	}
}
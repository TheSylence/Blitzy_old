using System;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class DelegateServiceMock : IViewService
	{
		public object Show( System.Windows.Window parent, object parameter = null )
		{
			return Action( parameter );
		}

		public Func<object, object> Action;
	}
}
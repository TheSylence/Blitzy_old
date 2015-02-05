

using System;
using Blitzy.Model;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class DataManipulationServiceMock<T> : IDataManipulationService where T : ModelBase
	{
		public Func<T> CreateFunc;
		public Func<T, bool> EditFunc;

		public Type ModelType
		{
			get { return typeof( T ); }
		}

		public object Create( System.Windows.Window parent )
		{
			if( CreateFunc != null )
			{
				return CreateFunc();
			}

			return null;
		}

		public bool Edit( System.Windows.Window parent, object obj )
		{
			if( EditFunc == null )
			{
				return false;
			}

			return EditFunc( obj as T );
		}
	}
}
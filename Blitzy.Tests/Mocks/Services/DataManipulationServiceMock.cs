// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.ViewServices;

namespace Blitzy.Tests.Mocks.Services
{
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
			return EditFunc( obj as T );
		}
	}
}
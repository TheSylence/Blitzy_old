// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Blitzy.ViewServices
{
	internal interface IDataManipulationService
	{
		Type ModelType { get; }

		object Create( Window parent );

		bool Edit( Window parent, object obj );
	}
}
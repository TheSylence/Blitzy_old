using System;
using System.Windows;

namespace Blitzy.ViewServices
{
	internal interface IDataManipulationService
	{
		object Create( Window parent );

		bool Edit( Window parent, object obj );

		Type ModelType { get; }
	}
}
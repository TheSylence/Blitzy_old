﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Blitzy.Controls
{
	/// <summary>
	/// Follow steps 1a or 1b and then 2 to use this custom control in a XAML file.
	///
	/// Step 1a) Using this custom control in a XAML file that exists in the current project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Blitzy.Controls"
	///
	///
	/// Step 1b) Using this custom control in a XAML file that exists in a different project.
	/// Add this XmlNamespace attribute to the root element of the markup file where it is
	/// to be used:
	///
	///     xmlns:MyNamespace="clr-namespace:Blitzy.Controls;assembly=Blitzy.Controls"
	///
	/// You will also need to add a project reference from the project where the XAML file lives
	/// to this project and Rebuild to avoid compilation errors:
	///
	///     Right click on the target project in the Solution Explorer and
	///     "Add Reference"->"Projects"->[Browse to and select this project]
	///
	///
	/// Step 2)
	/// Go ahead and use your control in the XAML file.
	///
	///     <MyNamespace:CommandListBox/>
	///
	/// </summary>
	[ExcludeFromCodeCoverage]
	public class CommandListView : ListView
	{
		#region Constructor

		static CommandListView()
		{
			//DefaultStyleKeyProperty.OverrideMetadata( typeof( CommandListBox ), new FrameworkPropertyMetadata( typeof( CommandListBox ) ) );
		}

		public CommandListView()
		{
			DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty( SelectedIndexProperty, typeof( ListView ) );
			if( dpd != null )
			{
				dpd.AddValueChanged( this, delegate
				{
					HandleSelectionChanged();
				} );
			}

			dpd = DependencyPropertyDescriptor.FromProperty( SelectedItemProperty, typeof( ListView ) );
			if( dpd != null )
			{
				dpd.AddValueChanged( this, delegate
				{
					HandleSelectionChanged();
				} );
			}
		}

		#endregion Constructor

		#region Methods

		private void HandleSelectionChanged()
		{
			ScrollIntoView( SelectedItem );
		}

		#endregion Methods
	}
}
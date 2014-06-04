using System;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.View.Dialogs
{
	/// <summary>
	/// Interaction logic for ExceptionDialog.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class ExceptionDialog : Window
	{
		public ExceptionDialog( Exception ex )
		{
			InitializeComponent();

			DataContext = new ExceptionDialogViewModel( ex );
		}
	}
}
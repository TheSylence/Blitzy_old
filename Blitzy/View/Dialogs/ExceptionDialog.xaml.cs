using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Blitzy.ViewModel.Dialogs;

namespace Blitzy.View.Dialogs
{
	/// <summary>
	/// Interaction logic for ExceptionDialog.xaml
	/// </summary>
	[ExcludeFromCodeCoverage]
	public partial class ExceptionDialog
	{
		public ExceptionDialog( Exception ex, StackTrace trace )
		{
			InitializeComponent();

			DataContext = new ExceptionDialogViewModel( ex, trace );
		}
	}
}
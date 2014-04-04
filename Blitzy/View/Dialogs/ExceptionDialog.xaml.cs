using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
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
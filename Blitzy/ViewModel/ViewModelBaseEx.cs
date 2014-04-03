// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GalaSoft.MvvmLight;

namespace Blitzy.ViewModel
{
	public class ViewModelBaseEx : ViewModelBase, IRequestCloseViewModel
	{
		#region Constructor

		#endregion Constructor

		#region Methods

		public virtual void Reset()
		{
		}

		protected void Close( bool? result = null )
		{
			if( RequestClose != null )
			{
				RequestClose( this, new CloseViewEventArgs( result ) );
			}
		}

		#endregion Methods

		#region Properties

		#endregion Properties

		#region Attributes

		#endregion Attributes

		#region Events

		public event EventHandler<CloseViewEventArgs> RequestClose;

		#endregion Events
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.View;
using Blitzy.ViewModel;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	public class CloseableView_Tests : TestBase, IRequestCloseViewModel
	{
		public event EventHandler<CloseViewEventArgs> RequestClose;

		public event EventHandler<EventArgs> RequestHide;

		[TestMethod, TestCategory( "ViewModel" )]
		public void CloseTest()
		{
			CloseableView v = new CloseableView();

			Assert.IsNull( RequestClose );
			v.DataContext = this;
			Assert.AreEqual( 1, RequestClose.GetInvocationList().Length );

			bool closed = false;
			v.Closed += ( s, e ) => closed = true;

			RequestClose( this, CloseViewEventArgs.Default );

			Assert.IsTrue( closed );
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void HideTest()
		{
			CloseableView v = new CloseableView();
			Assert.IsNull( RequestHide );
			v.DataContext = this;
			Assert.AreEqual( 1, RequestHide.GetInvocationList().Length );
		}
	}
}
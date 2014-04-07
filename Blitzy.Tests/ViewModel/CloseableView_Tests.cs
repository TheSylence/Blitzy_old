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

		public event EventHandler<EventArgs> RequestShow;

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
		public void ShowHideTest()
		{
			CloseableView v = new CloseableView();
			Assert.IsNull( RequestShow );
			v.DataContext = this;
			Assert.AreEqual( 1, RequestShow.GetInvocationList().Length );

			bool shown = false;
			v.Shown += ( s, e ) => shown = true;
			bool hidden = false;
			v.Hidden += ( s, e ) => hidden = true;

			RequestShow( this, EventArgs.Empty );
			RequestHide( this, EventArgs.Empty );

			Assert.IsTrue( shown );
			Assert.IsTrue( hidden );
		}
	}
}
// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Blitzy.Messages;
using Blitzy.Model;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.Model
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class UpdateChecker_Tests : TestBase
	{
		[TestMethod, TestCategory( "Model" )]
		public void UpdateCheckTest()
		{
			AutoResetEvent evt = new AutoResetEvent( false );
			bool showIfNewest = false;
			Messenger.Default.Register<VersionCheckMessage>( this, ( msg ) =>
			{
				showIfNewest = msg.ShowIfNewest;
				evt.Set();
			} );

			UpdateChecker checker = new UpdateChecker();
			checker.StartCheck( true );

			evt.WaitOne();
			Assert.IsTrue( showIfNewest );

			evt.Reset();
			checker.StartCheck( false );
			evt.WaitOne();
			Assert.IsFalse( showIfNewest );
		}
	}
}
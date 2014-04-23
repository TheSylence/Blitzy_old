// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewServices
{
	[TestClass] [System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class DialogServiceManager_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewServices" )]
		public void CreateTest()
		{
			DataManipulationServiceMock<Folder> mock = new DataManipulationServiceMock<Folder>();
			mock.CreateFunc = () => { return new Folder(); };

			DialogServiceManager.RegisterManipService( typeof( Folder ), mock );
			Assert.IsNotNull( DialogServiceManager.Create<Folder>() );
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void EditTest()
		{
			DataManipulationServiceMock<Folder> mock = new DataManipulationServiceMock<Folder>();
			mock.EditFunc = ( o ) => { o.ID = 123; return true; };

			DialogServiceManager.RegisterManipService( typeof( Folder ), mock );

			Folder f = new Folder();
			Assert.IsTrue( DialogServiceManager.Edit( f ) );
			Assert.AreEqual( 123, f.ID );
		}

		[TestMethod, TestCategory( "ViewServices" ), ExpectedException( typeof( ArgumentException ) )]
		public void InvalidCreateTest()
		{
			DialogServiceManager.Create<ModelBase>();
		}

		[TestMethod, TestCategory( "ViewServices" ), ExpectedException( typeof( ArgumentException ) )]
		public void InvalidEditTest()
		{
			Folder f = new Folder();
			DialogServiceManager.Edit( f );
		}

		[TestMethod, TestCategory( "ViewServices" ), ExpectedException( typeof( ArgumentException ) )]
		public void InvalidShowTest()
		{
			DialogServiceManager.Show<TextInputService>();
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void ShowTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			mock.Text = "test";

			DialogServiceManager.RegisterService( typeof( TextInputService ), mock );
			string result = DialogServiceManager.Show<TextInputService, string>();

			Assert.AreEqual( "test", result );
		}
	}
}
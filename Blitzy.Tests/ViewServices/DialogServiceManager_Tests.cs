using System;
using Blitzy.Model;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewServices;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewServices
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class DialogServiceManager_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewServices" )]
		public void CreateTest()
		{
			DataManipulationServiceMock<Folder> mock = new DataManipulationServiceMock<Folder>();
			mock.CreateFunc = () => { return new Folder(); };

			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterManipService( typeof( Folder ), mock );
			using( Folder folder = serviceManager.Create<Folder>() )
			{
				Assert.IsNotNull( folder );
			}
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void EditTest()
		{
			DataManipulationServiceMock<Folder> mock = new DataManipulationServiceMock<Folder>();
			mock.EditFunc = ( o ) => { o.ID = 123; return true; };

			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterManipService( typeof( Folder ), mock );

			using( Folder f = new Folder() )
			{
				Assert.IsTrue( serviceManager.Edit( f ) );
				Assert.AreEqual( 123, f.ID );
			}
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void InvalidCreateTest()
		{
			ViewServiceManager serviceManager = new ViewServiceManager();

			ExceptionAssert.Throws<ArgumentException>( () => serviceManager.Create<ModelBase>() );
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void InvalidEditTest()
		{
			ViewServiceManager serviceManager = new ViewServiceManager();
			using( Folder f = new Folder() )
			{
				ExceptionAssert.Throws<ArgumentException>( () => serviceManager.Edit( f ) );
			}
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void InvalidShowTest()
		{
			ViewServiceManager serviceManager = new ViewServiceManager();
			ExceptionAssert.Throws<ArgumentException>( () => serviceManager.Show<TextInputService>() );
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void ShowTest()
		{
			TextInputServiceMock mock = new TextInputServiceMock();
			mock.Value = "test";

			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );
			string result = serviceManager.Show<TextInputService, string>();

			Assert.AreEqual( "test", result );
		}

		[TestMethod, TestCategory( "ViewServices" )]
		public void UnregisterServiceTest()
		{
			CallCheckServiceMock mock = new CallCheckServiceMock();
			ViewServiceManager serviceManager = new ViewServiceManager();
			serviceManager.RegisterService( typeof( TextInputService ), mock );

			serviceManager.Show<TextInputService>();

			serviceManager.UnregisterService( typeof( TextInputService ) );
			ExceptionAssert.Throws<ArgumentException>( () => serviceManager.Show<TextInputService>() );
		}
	}
}
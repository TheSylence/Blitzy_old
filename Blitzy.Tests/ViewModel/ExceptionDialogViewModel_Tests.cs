// $Id$

using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using Blitzy.Tests.Mocks.Services;
using Blitzy.ViewModel.Dialogs;
using Blitzy.ViewServices;
using btbapi;
using Microsoft.QualityTools.Testing.Fakes;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Blitzy.Tests.ViewModel
{
	[TestClass]
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	public class ExceptionDialogViewModel_Tests : TestBase
	{
		[TestMethod, TestCategory( "ViewModel" )]
		public void ExitTest()
		{
			using( ExceptionDialogViewModel vm = new ExceptionDialogViewModel( new Exception(), new StackTrace( true ) ) )
			{
				bool closed = false;
				vm.RequestClose += ( s, e ) => closed = true;

				Assert.IsTrue( vm.ExitCommand.CanExecute( null ) );
				vm.ExitCommand.Execute( null );
				Assert.IsTrue( closed );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void PropertyChangedTest()
		{
			using( ExceptionDialogViewModel vm = new ExceptionDialogViewModel( new Exception(), new StackTrace( true ) ) )
			{
				PropertyChangedListener listener = new PropertyChangedListener( vm );
				listener.Exclude<ExceptionDialogViewModel>( v => v.ErrorReportText );
				Assert.IsTrue( listener.TestProperties() );
			}
		}

		[TestMethod, TestCategory( "ViewModel" )]
		public void SendTest()
		{
			Exception ex = new Exception( "This is a test" );
			using( ExceptionDialogViewModel vm = new ExceptionDialogViewModel( ex, new StackTrace( true ) ) )
			{
				bool closed = false;
				vm.RequestClose += ( s, e ) => closed = true;

				bool success = false;
				bool called = false;
				DelegateServiceMock mock = new DelegateServiceMock();
				mock.Action = ( args ) =>
					{
						called = true;
						success = ( (MessageBoxParameter)args ).Icon == MessageBoxImage.Information;
						return null;
					};

				DialogServiceManager.RegisterService( typeof( MessageBoxService ), mock );

				Assert.IsTrue( vm.SendCommand.CanExecute( null ) );
				using( ShimsContext.Create() )
				{
					btbapi.Fakes.ShimAPI.AllInstances.SendReportErrorReportStringVersion = ( api, report, software, version ) =>
					{
						return Task.Run<ErrorReportResult>( () =>
						{
							return new ErrorReportResult( System.Net.HttpStatusCode.OK, new Uri( "http://localhost/issue" ), false, null );
						} );
					};

					vm.SendCommand.Execute( null );
				}

				Assert.IsTrue( called );
				Assert.IsTrue( success );
				Assert.IsTrue( closed );

				called = false;
				closed = false;
				success = false;

				mock.Action = ( args ) =>
				{
					called = true;
					success = ( (MessageBoxParameter)args ).Icon == MessageBoxImage.Error;
					return null;
				};

				using( ShimsContext.Create() )
				{
					btbapi.Fakes.ShimAPI.AllInstances.SendReportErrorReportStringVersion = ( report, text, version, task ) =>
						{
							return Task.Run<btbapi.ErrorReportResult>( () =>
								{
									return new btbapi.ErrorReportResult( System.Net.HttpStatusCode.BadRequest, null, false, null );
								} );
						};

					vm.SendCommand.Execute( null );
					Assert.IsTrue( called );
					Assert.IsTrue( success );
					Assert.IsTrue( closed );
				}
			}
		}
	}
}
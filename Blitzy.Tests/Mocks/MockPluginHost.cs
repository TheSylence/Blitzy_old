using System;
using Blitzy.Model;
using Blitzy.Plugin;
using GalaSoft.MvvmLight.Messaging;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockPluginHost : IPluginHost
	{
		public MockPluginHost( Settings settings = null )
		{
			_Settings = settings;
		}

		public bool IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}

		public DbConnectionFactory ConnectionFactory
		{
			get { return new TestConnectionFactory(); }
		}

		public IMessenger Messenger { get; set; }

		public ISettings Settings
		{
			get { return _Settings; }
		}

		private Settings _Settings;
	}
}
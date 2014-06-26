// $Id$

using System;
using Blitzy.Model;
using Blitzy.Plugin;

namespace Blitzy.Tests.Mocks
{
	[System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]
	internal class MockPluginHost : IPluginHost
	{
		private Settings _Settings;
		private PluginDatabase ApiDatabase;

		public MockPluginHost( Settings settings = null )
		{
			_Settings = settings;

			if( _Settings != null )
			{
				ApiDatabase = new PluginDatabase( _Settings.Connection );
			}
		}

		public IDatabase Database
		{
			get
			{
				return ApiDatabase;
			}
		}

		public ISettings Settings
		{
			get { return _Settings; }
		}

		public bool IsPluginLoaded( Guid id )
		{
			throw new NotImplementedException();
		}
	}
}
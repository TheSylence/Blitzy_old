// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Plugin;

namespace Blitzy140
{
	internal class Plugin : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			throw new NotImplementedException();
		}

		public bool ExecuteCommand( Blitzy.Model.CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetCommands( IList<string> input )
		{
			throw new NotImplementedException();
		}

		public string GetInfo( IList<string> data, Blitzy.Model.CommandItem item )
		{
			throw new NotImplementedException();
		}

		public IPluginViewModel GetSettingsDataContext()
		{
			return new ViewModel();
		}

		public System.Windows.Controls.Control GetSettingsUI()
		{
			throw new NotImplementedException();
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetSubCommands( Blitzy.Model.CommandItem parent, IList<string> input )
		{
			throw new NotImplementedException();
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			throw new NotImplementedException();
		}

		public void Unload( PluginUnloadReason reason )
		{
			throw new NotImplementedException();
		}

		#endregion Methods

		#region Properties

		public int ApiVersion
		{
			get { return 1; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Compose tweets from Blitzy"; }
		}

		public bool HasSettings
		{
			get { return true; }
		}

		public string Name
		{
			get { return "Blitzy140"; }
		}

		public Guid PluginID
		{
			get { throw new NotImplementedException(); }
		}

		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public Uri Website
		{
			get { throw new NotImplementedException(); }
		}

		#endregion Properties

		#region Constants

		internal const string ConsumerKey = "***PUT KEY HERE***";
		internal const string ConsumerSecret = "***PUT KEY HERE***";
		internal IPluginHost Host;
		private const int MaxTweetLength = 140;

		#endregion Constants
	}
}
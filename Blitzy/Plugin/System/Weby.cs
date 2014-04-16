// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Blitzy.Model;

namespace Blitzy.Plugin.System
{
	internal class Weby : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			Items = null;
		}

		public bool ExecuteCommand( CommandItem command, IList<string> input, out string message )
		{
			string url;
			Uri uri;
			CultureInfo culture = CultureInfo.CurrentUICulture;
			string term = input[1];

			// weby
			if( command.UserData == null )
			{
				url = ( term.StartsWith( "http://", true, culture ) ||
					term.StartsWith( "ftp://", true, culture ) ||
					term.StartsWith( "https://", true, culture ) ||
					term.StartsWith( "file://", true, culture ) )
					? PrepareString( term ) : string.Format( culture, "http://{0}", term );
				uri = new Uri( url );
			}
			// user defined
			else
			{
				url = command.UserData as string;
				uri = new Uri( string.Format( culture, url, PrepareString( term ) ) );
			}

			Process.Start( uri.AbsoluteUri );
			message = null;
			return true;
		}

		public IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( Items == null )
			{
				Items = new List<CommandItem>();

				Items.Add( CommandItem.Create( "weby", "OpenWebsite".Localize(), this, "Weby.png" ) );

				// FIXME: Rewrite for correct API use
				//using( DbCommand cmd = Host.Database.CreateCommand() )
				//{
				//	cmd.CommandText = "SELECT Name, Description, URL, Icon FROM weby_websites";
				//	using( DbDataReader reader = cmd.ExecuteReader() )
				//	{
				//		while( reader.Read() )
				//		{
				//			string url = reader.GetString( 2 );
				//			string name = reader.GetString( 0 );
				//			string desc = reader.GetString( 1 );
				//			string icon = reader.GetString( 3 );
				//			if( string.IsNullOrWhiteSpace( icon ) )
				//			{
				//				icon = "Weby.png";
				//			}

				//			Items.Add( CommandItem.Create( name, desc, this, icon, url ) );
				//		}
				//	}
				//}
			}

			return Items;
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield return parent;
		}

		[global::System.Diagnostics.CodeAnalysis.SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
		public bool Load( IPluginHost host, string oldVersion = null )
		{
			/*	*/

			Host = host;

			if( oldVersion == null )
			{
				// FIXME: Rewrite for correct API use
				//string[] engines = new[]
				//{
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES (1, 'google', 'https://www.google.com/search?source=Blitzy&q={0}', 'Search the internet using Google', 'https://www.google.de/images/google_favicon_128.png');",
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES (2, 'wiki', 'http://en.wikipedia.org/wiki/{0}', 'Search in wikipedia (en)', 'http://bits.wikimedia.org/favicon/wikipedia.ico');",
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES( 3, 'youtube', 'http://www.youtube.com/results?search_query={0}', 'Search in YouTube', 'http://s.ytimg.com/yts/img/favicon-vfldLzJxy.ico');",
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES (4, 'bing', 'http://www.bing.com/search?q={0}', 'Seach the internet using Bing', 'http://www.bing.com/s/a/bing_p.ico');",
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES (5, 'facebook', 'https://www.facebook.com/search/results.php?q={0}', 'Search in facebook', 'https://fbstatic-a.akamaihd.net/rsrc.php/yl/r/H3nktOa7ZMg.ico');",
				//	"INSERT INTO weby (WebyID, Name, Url, Description, Icon) VALUES (6, 'wolfram', 'http://www.wolframalpha.com/input/?i={0}', 'Compute something using Wolfram Alpha', 'http://www.wolframalpha.com/favicon_calculate.ico');"
				//};

				//DbTransaction transaction = Host.Database.BeginTransaction();
				//try
				//{
				//	foreach( string eng in engines )
				//	{
				//		using( DbCommand cmd = Host.Database.CreateCommand() )
				//		{
				//			cmd.CommandText = eng;
				//			cmd.Transaction = transaction;
				//			cmd.ExecuteNonQuery();
				//		}
				//	}

				//	transaction.Commit();
				//}
				//catch
				//{
				//	transaction.Rollback();
				//	throw;
				//}
			}

			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
		}

		private static string PrepareString( string term )
		{
			return HttpUtility.UrlEncode( term );
		}

		#endregion Methods

		#region Constants

		internal const string GuidString = "01CB38CB-A064-4AE7-9B35-28F3FE65416E";

		#endregion Constants

		#region Properties

		private Guid? GUID;
		private IPluginHost Host;
		private List<CommandItem> Items;

		public int ApiVersion
		{
			get { return Constants.APIVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Open websites or query search engines using Blitzy"; }
		}

		public string Name
		{
			get { return "Weby"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !GUID.HasValue )
				{
					GUID = Guid.Parse( GuidString );
				}

				return GUID.Value;
			}
		}

		public string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		#endregion Properties
	}
}
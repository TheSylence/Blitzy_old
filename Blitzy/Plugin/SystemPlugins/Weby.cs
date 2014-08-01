// $Id$

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Web;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Weby : IPlugin
	{
		#region Methods

		public void ClearCache()
		{
			Items = null;
		}

		public bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			string url;
			Uri uri;
			CultureInfo culture = CultureInfo.CurrentUICulture;
			string term = input[1];

			// weby
			if( command.UserData == null )
			{
				url = IsUrl( term ) ? PrepareString( term ) : string.Format( culture, "http://{0}", term );
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
				Items = new List<CommandItem>
				{
					CommandItem.Create( "weby", "OpenWebsite".Localize(), this, "Weby.png" )
				};

				IEnumerable<IDictionary<string, object>> sites = Host.Database.Select( this, "websites", new[] { "Name", "Description", "URL", "Icon" } );
				foreach( IDictionary<string, object> site in sites )
				{
					string url = site["Url"].ToString();
					string name = site["Name"].ToString();
					string desc = site["Description"].ToString();
					string icon = site["Icon"].ToString();
					if( string.IsNullOrWhiteSpace( icon ) )
					{
						icon = "Weby.png";
					}

					Items.Add( CommandItem.Create( name, desc, this, icon, url, null, null, true ) );
				}
			}

			return Items;
		}

		public string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public IPluginViewModel GetSettingsDataContext()
		{
			return new ViewModel.WebySettingsViewModel( (Settings)Host.Settings );
		}

		public System.Windows.Controls.Control GetSettingsUI()
		{
			return new WebyUI();
		}

		public IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield return parent;
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;

			if( oldVersion == null )
			{
				TableColumn[] columns =
				{
					new TableColumn( "WebyID", ColumnType.Numeric ),
					new TableColumn( "Name", ColumnType.Text, 50 ),
					new TableColumn( "Description", ColumnType.Text, 255 ),
					new	TableColumn( "Url", ColumnType.Text ),
					new TableColumn( "Icon", ColumnType.Text )
				};
				if( !Host.Database.CreateTable( this, "websites", columns ) )
				{
					return false;
				}

				Dictionary<string, object>[] values =
				{
					new Dictionary<string,object>{ {"WebyID", 1 }, {"Name", "google" }, {"Url", "https://www.google.com/search?source=Blitzy&q={0}" }, {"Description", "Search the internet using Google"}, {"Icon", "https://www.google.de/images/google_favicon_128.png" } },
					new Dictionary<string,object>{ {"WebyID", 2 }, {"Name", "wiki" }, {"Url", "http://en.wikipedia.org/wiki/{0}" }, {"Description", "Search in wikipedia (en)" }, {"Icon", "http://bits.wikimedia.org/favicon/wikipedia.ico"} },
					new Dictionary<string,object>{ {"WebyID", 3 }, {"Name", "youtube" }, {"Url", "http://www.youtube.com/results?search_query={0}" }, {"Description", "Search in YouTube" }, {"Icon", "http://s.ytimg.com/yts/img/favicon-vfldLzJxy.ico" } },
					new Dictionary<string,object>{ {"WebyID", 4 }, {"Name", "bing" }, {"Url", "http://www.bing.com/search?q={0}" }, {"Description", "Seach the internet using Bing" }, {"Icon", "http://www.bing.com/s/a/bing_p.ico" } },
					new Dictionary<string,object>{ {"WebyID", 5 }, {"Name", "facebook" }, {"Url", "https://www.facebook.com/search/results.php?q={0}"}, {"Description", "Search in facebook" }, {"Icon", "https://fbstatic-a.akamaihd.net/rsrc.php/yl/r/H3nktOa7ZMg.ico" } },
					new Dictionary<string,object>{ {"WebyID", 6 }, {"Name", "wolfram" }, {"Url", "http://www.wolframalpha.com/input/?i={0}" }, {"Description", "Compute something using Wolfram Alpha" }, {"Icon", "http://www.wolframalpha.com/favicon_calculate.ico"} }
				};

				DbTransaction transaction = Host.Database.BeginTransaction();
				try
				{
					if( Host.Database.Insert( this, "websites", values ) != values.Count() )
					{
						transaction.Rollback();
						return false;
					}
					transaction.Commit();
				}
				catch
				{
					transaction.Rollback();
					throw;
				}
			}

			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
		}

		internal static bool IsUrl( string term )
		{
			CultureInfo culture = CultureInfo.CurrentUICulture;
			return ( term.StartsWith( "http://", true, culture ) ||
					term.StartsWith( "ftp://", true, culture ) ||
					term.StartsWith( "https://", true, culture ) ||
					term.StartsWith( "file://", true, culture ) );
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

		private Guid? Guid;
		private IPluginHost Host;
		private List<CommandItem> Items;

		public int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public string Author
		{
			get { return "Matthias Specht"; }
		}

		public string Description
		{
			get { return "Open websites or query search engines using Blitzy"; }
		}

		public bool HasSettings { get { return true; } }

		public string Name
		{
			get { return "Weby"; }
		}

		public Guid PluginID
		{
			get
			{
				if( !Guid.HasValue )
				{
					Guid = global::System.Guid.Parse( GuidString );
				}

				return Guid.Value;
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
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Reflection;
using System.Web;
using Blitzy.Model;
using Blitzy.Utility;

namespace Blitzy.Plugin.SystemPlugins
{
	internal class Weby : InternalPlugin
	{
		public override void ClearCache()
		{
			Items = null;
		}

		public override bool ExecuteCommand( CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
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

		public override IEnumerable<CommandItem> GetCommands( IList<string> input )
		{
			if( Items == null )
			{
				Items = new List<CommandItem>
				{
					CommandItem.Create( "weby", "OpenWebsite".Localize(), this, "Weby.png" )
				};

				using( DbConnection connection = Host.ConnectionFactory.OpenConnection() )
				{
					using( DbCommand cmd = connection.CreateCommand() )
					{
						cmd.CommandText = "SELECT Name, Description, URL, Icon FROM weby_websites;";

						using( DbDataReader reader = cmd.ExecuteReader() )
						{
							while( reader.Read() )
							{
								string url = reader.ReadString( "Url" );
								string name = reader.ReadString( "Name" );
								string desc = reader.ReadString( "Description" );
								string icon = reader.ReadString( "Icon" );
								if( string.IsNullOrWhiteSpace( icon ) )
								{
									icon = "Weby.png";
								}

								Items.Add( CommandItem.Create( name, desc, this, icon, url, null, null, true ) );
							}
						}
					}
				}
			}

			return Items;
		}

		public override string GetInfo( IList<string> data, CommandItem item )
		{
			return null;
		}

		public override IPluginViewModel GetSettingsDataContext( IViewServiceManager viewServices )
		{
			return new ViewModel.WebySettingsViewModel( Host.ConnectionFactory, (Settings)Host.Settings, viewServices );
		}

		public override System.Windows.Controls.Control GetSettingsUI()
		{
			return new WebyUI();
		}

		public override IEnumerable<CommandItem> GetSubCommands( CommandItem parent, IList<string> input )
		{
			yield return parent;
		}

		[SuppressMessage( "Microsoft.Security", "CA2100:Review SQL queries for security vulnerabilities" )]
		public override bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;

			if( oldVersion == null )
			{
				using( DbConnection connection = host.ConnectionFactory.OpenConnection() )
				{
					DbTransaction tx = connection.BeginTransaction();
					try
					{
						using( DbCommand cmd = connection.CreateCommand() )
						{
							cmd.CommandText = QueryBuilder.CreateTable( "weby_websites", new Dictionary<string, string>
						{
							{ "WebyID", "INTEGER PRIMARY KEY" },
							{ "Name", "VARCHAR(50) NOT NULL" },
							{ "Description", "VARCHAR(255) NOT NULL" },
							{ "Url", "TEXT NOT NULL" },
							{ "Icon", "TEXT" }
						} );

							cmd.ExecuteNonQuery();
						}

						List<WebyWebsite> defaultSites = new List<WebyWebsite>();
						defaultSites.Add( new WebyWebsite { ID = 1, Name = "google", URL = "https://www.google.com/search?source=Blitzy&q={0}", Description = "Search the internet using Google", Icon = "https://www.google.de/images/google_favicon_128.png" } );
						defaultSites.Add( new WebyWebsite { ID = 2, Name = "wiki", URL = "http://en.wikipedia.org/wiki/{0}", Description = "Search in wikipedia (en)", Icon = "http://bits.wikimedia.org/favicon/wikipedia.ico" } );
						defaultSites.Add( new WebyWebsite { ID = 3, Name = "youtube", URL = "http://www.youtube.com/results?search_query={0}", Description = "Search in YouTube", Icon = "http://s.ytimg.com/yts/img/favicon-vfldLzJxy.ico" } );
						defaultSites.Add( new WebyWebsite { ID = 4, Name = "bing", URL = "http://www.bing.com/search?q={0}", Description = "Seach the internet using Bing", Icon = "http://www.bing.com/s/a/bing_p.ico" } );
						defaultSites.Add( new WebyWebsite { ID = 5, Name = "facebook", URL = "https://www.facebook.com/search/results.php?q={0}", Description = "Search in facebook", Icon = "https://fbstatic-a.akamaihd.net/rsrc.php/yl/r/H3nktOa7ZMg.ico" } );
						defaultSites.Add( new WebyWebsite { ID = 6, Name = "wolfram", URL = "http://www.wolframalpha.com/input/?i={0}", Description = "Compute something using Wolfram Alpha", Icon = "http://www.wolframalpha.com/favicon_calculate.ico" } );

						foreach( WebyWebsite site in defaultSites )
						{
							site.Save( connection );
						}

						tx.Commit();
					}
					catch
					{
						tx.Rollback();
						return false;
					}
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
			}

			return true;
		}

		public override void Unload( PluginUnloadReason reason )
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

		public override int ApiVersion
		{
			get { return Constants.ApiVersion; }
		}

		public override string Author
		{
			get { return "Matthias Specht"; }
		}

		public override string Description
		{
			get { return "Open websites or query search engines using Blitzy"; }
		}

		public override bool HasSettings { get { return true; } }

		public override string Name
		{
			get { return "Weby"; }
		}

		public override Guid PluginID
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

		public override string Version
		{
			get { return Assembly.GetExecutingAssembly().GetName().Version.ToString(); }
		}

		public override Uri Website
		{
			get { return new Uri( "http://btbsoft.org" ); }
		}

		internal const string GuidString = "01CB38CB-A064-4AE7-9B35-28F3FE65416E";

		private Guid? Guid;
		private IPluginHost Host;
		private List<CommandItem> Items;
	}
}
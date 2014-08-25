// $Id$

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Blitzy.Model;
using Blitzy.Plugin;
using LinqToTwitter;

namespace Blitzy140
{
	// TODO: This thing needs localization
	internal class Plugin : IPlugin
	{
		#region Methods

		private List<CommandItem> Items;
		private Dictionary<string, Tuple<string, string>> KeyStore;

		public void ClearCache()
		{
			KeyStore = null;
			Items = null;
		}

		public bool ExecuteCommand( Blitzy.Model.CommandItem command, CommandExecutionMode mode, IList<string> input, out string message )
		{
			string status = "";
			string account = command.UserData as string;

			if( input.Count == 1 )
			{
				status = input[0];
			}
			else if( input.Count == 2 )
			{
				account = input[0];
				status = input[1];
			}
			// Don't send empty tweets
			if( string.IsNullOrEmpty( status ) )
			{
				message = string.Empty;
				return true;
			}

			// TODO: Read tokens from database for selected user
			string token = "";
			string tokenSecret = "";

			// See https://linqtotwitter.codeplex.com/discussions/261363
			SingleUserAuthorizer auth = new SingleUserAuthorizer()
			{
				CredentialStore = new InMemoryCredentialStore()
				{
					OAuthToken = token,
					OAuthTokenSecret = tokenSecret,
					ConsumerKey = GetKey( ApiKey ),
					ConsumerSecret = GetKey( ApiSecret )
				}
			};

			using( TwitterContext ctx = new TwitterContext( auth ) )
			{
				Task<Status> task = ctx.TweetAsync( status );
				task.Wait();
				Status tweet = task.Result;

				if( tweet != null )
				{
					// Success
					message = null;
					return true;
				}
				else
				{
					// Error
					message = "There was an error";
					return false;
				}
			}
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetCommands( IList<string> input )
		{
			if( Items == null )
			{
				KeyStore = new Dictionary<string, Tuple<string, string>>();

				Items = new List<CommandItem>();
				const string icon = "";

				CommandItem root = CommandItem.Create( "tweet", "Post a Tweet to Twitter", this, icon );
				Items.Add( root );

				IEnumerable<IDictionary<string, object>> accounts = Host.Database.Select( this, "accounts", new[] { "UserName", "AuthToken", "AuthSecret" } );
				foreach( IDictionary<string, object> account in accounts )
				{
					string displayName = account["UserName"].ToString();
					string token = account["AuthToken"].ToString();
					string tokenSecrent = account["AuthSecret"].ToString();
					string description = string.Format( "Tweet on account {0}", displayName );

					Items.Add( CommandItem.Create( displayName, description, this, icon, null, root, null, true ) );
					KeyStore.Add( displayName, new Tuple<string, string>( token, tokenSecrent ) );
				}
			}

			return Items;
		}

		public string GetInfo( IList<string> data, Blitzy.Model.CommandItem item )
		{
			return string.Format( CultureInfo.CurrentUICulture, "{0} characters left.", ( MaxTweetLength - data.Last().Length ) );
		}

		public IPluginViewModel GetSettingsDataContext()
		{
			return new ViewModel();
		}

		public System.Windows.Controls.Control GetSettingsUI()
		{
			return new PluginUI();
		}

		public IEnumerable<Blitzy.Model.CommandItem> GetSubCommands( Blitzy.Model.CommandItem parent, IList<string> input )
		{
			yield return parent;
		}

		public bool Load( IPluginHost host, string oldVersion = null )
		{
			Host = host;

			if( oldVersion == null )
			{
				TableColumn[] columns = new TableColumn[]
				{
					new TableColumn( "UserName", ColumnType.Text ),
					new TableColumn( "AuthToken", ColumnType.Text ),
					new TableColumn( "AuthSecret", ColumnType.Text ),
					new TableColumn( "IsDefault", ColumnType.Integer, 0, true )
				};

				if( !Host.Database.CreateTable( this, "accounts", columns ) )
				{
					return false;
				}
			}

			return true;
		}

		public void Unload( PluginUnloadReason reason )
		{
		}

		private static string GetKey( string key )
		{
			return string.Join( string.Empty, key.ToArray().Reverse() );
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

		internal const string ApiKey = "htNGykuE5Yw1oB5ZDmdSpZwqK";
		internal const string ApiSecret = "qRZwpP0pnlCSBiKb2AN4iSaCfTvIWj615QWxg8PoJS2loCchb8";
		internal IPluginHost Host;
		private const int MaxTweetLength = 140;

		#endregion Constants
	}
}
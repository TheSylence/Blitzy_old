using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Blitzy.btbapi
{
	public class API
	{
		public API( APIEndPoint endpoint )
		{
			EndPoint = endpoint;
		}

		public async Task<VersionInfo> CheckVersion( string product, Version currentVersion = null, bool includePreReleases = false )
		{
			Dictionary<string, string> args = new Dictionary<string, string>();
			args.Add( "projectname", product );
			if( currentVersion != null )
			{
				args.Add( "currentversion", currentVersion.ToString() );
			}
			if( includePreReleases )
			{
				args.Add( "prerelease", true.ToString() );
			}

			string response = null;
			using( HttpClient client = new HttpClient() )
			{
				string requestUrl = APIHelper.BuildUrl( EndPoint, APIHelper.UpdateCheckAction, args );
				HttpResponseMessage msg;
				try
				{
					msg = await client.GetAsync( requestUrl );
				}
				catch( HttpRequestException )
				{
					return new VersionInfo( HttpStatusCode.BadRequest, null, null, null, 0, null );
				}

				response = await msg.Content.ReadAsStringAsync();
				try
				{
					msg.EnsureSuccessStatusCode();
				}
				catch( HttpRequestException )
				{
					return new VersionInfo( msg.StatusCode, null, null, null, 0, null );
				}
			}

			Dictionary<string, object> data =
				JsonConvert.DeserializeObject<Dictionary<string, object>>( response );

			Uri downloadLink = null;
			string md5 = null;
			long size = 0;
			if( data["download"] != null )
			{
				downloadLink = new Uri( data["download"].ToString() );
				md5 = data["md5"].ToString();
				size = Convert.ToInt64( data["size"] );
			}

			Version version;
			if( data["version"] != null )
			{
				version = new Version( data["version"].ToString() );
			}
			else
			{
				version = new Version();
			}

			Dictionary<string, string> changelog;
			if( data["changelog"] != null )
			{
				changelog = JsonConvert.DeserializeObject<Dictionary<string, string>>( data["changelog"].ToString() );
			}
			else
			{
				changelog = new Dictionary<string, string>();
			}

			Dictionary<Version, string> changes = new Dictionary<Version, string>();
			foreach( KeyValuePair<string, string> kvp in changelog )
			{
				changes.Add( new Version( kvp.Key ), kvp.Value );
			}

			return new VersionInfo( HttpStatusCode.OK, version, downloadLink, md5, size, changes );
		}

		public async Task<ErrorReportResult> SendReport( ErrorReport report, string product, Version currentVersion )
		{
			using( HttpClient client = new HttpClient() )
			{
				Dictionary<string, string> values = new Dictionary<string, string>();
				values.Add( "project", product );
				values.Add( "version", currentVersion.ToString() );
				values.Add( "title", report.Exception.Message );
				values.Add( "hash", report.GetHash() );
				values.Add( "exception", report.ToString() );

				FormUrlEncodedContent content = new FormUrlEncodedContent( values );
				HttpResponseMessage msg;
				try
				{
					msg = await client.PostAsync( APIHelper.BuildUrl( EndPoint, APIHelper.ErrorReportAction, new Dictionary<string, string>() ), content );
				}
				catch( HttpRequestException )
				{
					return new ErrorReportResult( HttpStatusCode.BadRequest, null, false );
				}

				string response = await msg.Content.ReadAsStringAsync();
				try
				{
					msg.EnsureSuccessStatusCode();
				}
				catch( HttpRequestException )
				{
					return new ErrorReportResult( msg.StatusCode, null, false );
				}

				Dictionary<string, string> results = JsonConvert.DeserializeObject<Dictionary<string, string>>( response );

				return new ErrorReportResult( HttpStatusCode.OK, new Uri( results["url"] ), Convert.ToBoolean( results["already_known"] ) );
			}
		}

		protected APIEndPoint EndPoint { get; private set; }
	}
}
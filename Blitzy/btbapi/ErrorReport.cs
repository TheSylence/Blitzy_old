using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.btbapi
{
	/// <summary>
	/// An error report that can be used to send information about an error.
	/// </summary>
	public class ErrorReport
	{
		/// <summary>
		/// Constructs a new report
		/// </summary>
		/// <param name="exception">The exception to be reported</param>
		/// <param name="trace">The stacktrace. Remember to capture file info with <code>new StackFrame(true)</code></param>
		public ErrorReport( Exception exception, StackTrace trace )
		{
			Exception = exception;
			Trace = trace;

			AppendAssemblies();
		}

		/// <summary>
		/// Adds additional data to the report
		/// </summary>
		/// <param name="name">Name of the data</param>
		/// <param name="data">The data</param>
		public void AttachData( string name, string data )
		{
			AdditionalData.Add( name, data );
		}

		/// <summary>
		/// Attaches a file to this report
		/// </summary>
		/// <param name="fileName">Name of the file to attach</param>
		public void AttachFile( string fileName )
		{
			using( TextReader reader = new StreamReader( fileName ) )
			{
				AttachData( fileName, reader.ReadToEnd() );
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendLine( Exception.ToString() );
			sb.AppendLine();

			sb.AppendLine( "=============================================" );
			sb.AppendLine( "Trace" );
			sb.AppendLine( "=============================================" );

			foreach( StackFrame frame in Trace.GetFrames() )
			{
				MethodBase method = frame.GetMethod();
				sb.AppendFormat( "{0}.{1}", method.DeclaringType.Name, method.Name );
				if( method.GetParameters().Count() > 0 )
				{
					sb.Append( "( " );
					sb.Append( string.Join( ", ", method.GetParameters().Select( p => p.ParameterType.Name + " " + p.Name ) ) );
					sb.Append( " ) in " );
				}
				else
				{
					sb.Append( "() in " );
				}

				string fileName = frame.GetFileName();
				if( string.IsNullOrWhiteSpace( fileName ) )
				{
					fileName = "<unknown>";
				}
				sb.AppendFormat( "{0}:{1}", fileName, frame.GetFileLineNumber(), frame.GetFileColumnNumber() );
				sb.AppendLine();
			}

			foreach( KeyValuePair<string, string> kvp in AdditionalData )
			{
				sb.AppendLine();
				sb.AppendLine( "=============================================" );
				sb.AppendLine( kvp.Key );
				sb.AppendLine( "=============================================" );
				sb.AppendLine( kvp.Value );
			}

			return sb.ToString();
		}

		internal string GetHash()
		{
			StringBuilder sb = new StringBuilder();
			sb.Append( Exception.Message );
			StackFrame frame = Trace.GetFrame( 0 );

			sb.Append( frame.GetFileName() );
			sb.Append( frame.GetFileLineNumber() );
			sb.Append( Trace.FrameCount );

			return APIHelper.MD5( sb.ToString() );
		}

		private void AppendAssemblies()
		{
			StringBuilder sb = new StringBuilder();

			foreach( Assembly asm in AppDomain.CurrentDomain.GetAssemblies() )
			{
				sb.AppendLine( asm.FullName );
			}

			AttachData( "Loaded Assemblies", sb.ToString() );
		}

		internal Dictionary<string, string> AdditionalData = new Dictionary<string, string>();
		internal Exception Exception;
		internal StackTrace Trace;
	}
}
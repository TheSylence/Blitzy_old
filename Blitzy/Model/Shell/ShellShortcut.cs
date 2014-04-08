// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Blitzy.Model.Shell
{
	/// <remarks>
	///   .NET friendly wrapper for the ShellLink class
	/// </remarks>
	public class ShellShortcut : IDisposable
	{
		private const int INFOTIPSIZE = 1024;
		private const int MAX_PATH = 260;

		private const int SW_SHOWMAXIMIZED = 3;
		private const int SW_SHOWMINIMIZED = 2;
		private const int SW_SHOWMINNOACTIVE = 7;
		private const int SW_SHOWNORMAL = 1;

		private IShellLinkW m_Link;

		private string m_sPath;

		///
		/// <param name='linkPath'>
		///   Path to new or existing shortcut file (.lnk).
		/// </param>
		///
		public ShellShortcut( string linkPath )
		{
			IPersistFile pf;

			m_sPath = linkPath;

			m_Link = (IShellLinkW)new ShellLink();

			if( File.Exists( linkPath ) )
			{
				pf = (IPersistFile)m_Link;
				pf.Load( linkPath, 0 );
			}
		}

		/// <value>
		///   Gets or sets the argument list of the shortcut.
		/// </value>
		public string Arguments
		{
			get
			{
				StringBuilder sb = new StringBuilder( INFOTIPSIZE );
				m_Link.GetArguments( sb, sb.Capacity );
				return sb.ToString();
			}
			set { m_Link.SetArguments( value ); }
		}

		/// <value>
		///   Gets or sets a description of the shortcut.
		/// </value>
		public string Description
		{
			get
			{
				StringBuilder sb = new StringBuilder( INFOTIPSIZE );
				m_Link.GetDescription( sb, sb.Capacity );
				return sb.ToString();
			}
			set { m_Link.SetDescription( value ); }
		}

		/// <value>
		///   Retrieves the Icon of the shortcut as it will appear in Explorer.
		///   Use the <see cref="IconPath"/> and <see cref="IconIndex"/>
		///   properties to change it.
		/// </value>
		public Icon Icon
		{
			get
			{
				StringBuilder sb = new StringBuilder( MAX_PATH );
				int nIconIdx;
				IntPtr hIcon, hInst;
				Icon ico, clone;

				m_Link.GetIconLocation( sb, sb.Capacity, out nIconIdx );
				hInst = Marshal.GetHINSTANCE( this.GetType().Module );
				hIcon = Native.ExtractIcon( hInst, sb.ToString(), nIconIdx );
				if( hIcon == IntPtr.Zero )
					return null;

				// Return a cloned Icon, because we have to free the original ourselves.
				ico = Icon.FromHandle( hIcon );
				clone = (Icon)ico.Clone();
				ico.Dispose();
				Native.DestroyIcon( hIcon );
				return clone;
			}
		}

		/// <value>
		///   Gets or sets the index of the <see cref="Icon"/> assigned to the shortcut.
		///   Set to zero when the <see cref="IconPath"/> property specifies a .ICO file.
		/// </value>
		/// <summary>
		///   <seealso cref="IconPath"/>
		/// </summary>
		public int IconIndex
		{
			get
			{
				StringBuilder sb = new StringBuilder( MAX_PATH );
				int nIconIdx;
				m_Link.GetIconLocation( sb, sb.Capacity, out nIconIdx );
				return nIconIdx;
			}
			set { m_Link.SetIconLocation( IconPath, value ); }
		}

		/// <value>
		///   Gets or sets the path of the <see cref="Icon"/> assigned to the shortcut.
		/// </value>
		/// <summary>
		///   <seealso cref="IconIndex"/>
		/// </summary>
		public string IconPath
		{
			get
			{
				StringBuilder sb = new StringBuilder( MAX_PATH );
				int nIconIdx;
				m_Link.GetIconLocation( sb, sb.Capacity, out nIconIdx );
				return sb.ToString();
			}
			set { m_Link.SetIconLocation( value, IconIndex ); }
		}

		//
		// If Path returns an empty string, the shortcut is associated with
		// a PIDL instead, which can be retrieved with IShellLink.GetIDList().
		// This is beyond the scope of this wrapper class.
		//
		/// <value>
		///   Gets or sets the target path of the shortcut.
		/// </value>
		public string Path
		{
			get
			{
				WIN32_FIND_DATAW wfd = new WIN32_FIND_DATAW();
				StringBuilder sb = new StringBuilder( MAX_PATH );

				m_Link.GetPath( sb, sb.Capacity, out wfd, SLGP_FLAGS.SLGP_UNCPRIORITY );
				return ShellLinkHelper.ResolveX64Path( sb.ToString() );
			}
			set { m_Link.SetPath( value ); }
		}

		/// <summary>
		///   Returns a reference to the internal ShellLink object,
		///   which can be used to perform more advanced operations
		///   not supported by this wrapper class, by using the
		///   IShellLink interface directly.
		/// </summary>
		public object ShellLink
		{
			get { return m_Link; }
		}

		/// <value>
		///   Gets or sets the System.Diagnostics.ProcessWindowStyle value
		///   that decides the initial show state of the shortcut target. Note that
		///   ProcessWindowStyle.Hidden is not a valid property value.
		/// </value>
		public ProcessWindowStyle WindowStyle
		{
			get
			{
				int nWS;
				m_Link.GetShowCmd( out nWS );

				switch( nWS )
				{
					case SW_SHOWMINIMIZED:
					case SW_SHOWMINNOACTIVE:
						return ProcessWindowStyle.Minimized;

					case SW_SHOWMAXIMIZED:
						return ProcessWindowStyle.Maximized;

					default:
						return ProcessWindowStyle.Normal;
				}
			}
			set
			{
				int nWS;

				switch( value )
				{
					case ProcessWindowStyle.Normal:
						nWS = SW_SHOWNORMAL;
						break;

					case ProcessWindowStyle.Minimized:
						nWS = SW_SHOWMINNOACTIVE;
						break;

					case ProcessWindowStyle.Maximized:
						nWS = SW_SHOWMAXIMIZED;
						break;

					default: // ProcessWindowStyle.Hidden
						throw new ArgumentException( "Unsupported ProcessWindowStyle value." );
				}

				m_Link.SetShowCmd( nWS );
			}
		}

		/// <value>
		///   Gets or sets the working directory (aka start in directory) of the shortcut.
		/// </value>
		public string WorkingDirectory
		{
			get
			{
				StringBuilder sb = new StringBuilder( MAX_PATH );
				m_Link.GetWorkingDirectory( sb, sb.Capacity );
				return sb.ToString();
			}
			set { m_Link.SetWorkingDirectory( value ); }
		}

		//
		//  IDisplosable implementation
		//
		public void Dispose()
		{
			if( m_Link != null )
			{
				Marshal.ReleaseComObject( m_Link );
				m_Link = null;
			}
		}

		/// <summary>
		///   Saves the shortcut to disk.
		/// </summary>
		public bool Save()
		{
			IPersistFile pf = (IPersistFile)m_Link;
			return pf.Save( m_sPath, true ) == 0;
		}

		#region Native Win32 API functions

		private class Native
		{
			[DllImport( "user32.dll" )]
			public static extern bool DestroyIcon( IntPtr hIcon );

			[DllImport( "shell32.dll", CharSet = CharSet.Auto )]
			public static extern IntPtr ExtractIcon( IntPtr hInst, string lpszExeFileName, int nIconIndex );
		}

		#endregion Native Win32 API functions
	}
}
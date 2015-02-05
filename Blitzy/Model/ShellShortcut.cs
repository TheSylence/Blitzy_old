

/**************************************************************************
*
* Filename:     ShellShortcut.cs
* Author:       Mattias Sjögren (mattias@mvps.org)
*               http://www.msjogren.net/dotnet/
*
* Description:  Defines a .NET friendly class, ShellShortcut, for reading
*               and writing shortcuts.
*               Define the conditional compilation symbol UNICODE to use
*               IShellLinkW internally.
*
* Public types: class ShellShortcut
*
*
* Dependencies: ShellLinkNative.cs
*
*
* Copyright ©2001-2002, Mattias Sjögren
*
**************************************************************************/
#define UNICODE

using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Runtime.ExceptionServices;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Windows.Forms;

namespace Blitzy.Model
{
	public class ShellShortcut
	{
		#region Constructor

		public ShellShortcut( string linkPath )
		{
			if( !System.IO.Path.IsPathRooted( linkPath ) )
			{
				linkPath = System.IO.Path.Combine( Directory.GetCurrentDirectory(), linkPath );
			}

			string pathOnly = System.IO.Path.GetDirectoryName( linkPath );
			string filenameOnly = System.IO.Path.GetFileName( linkPath );

			Shell32.Shell shell = new Shell32.Shell();
			Shell32.Folder folder = shell.NameSpace( pathOnly );
			Shell32.FolderItem folderItem = folder.ParseName( filenameOnly );
			if( folderItem != null )
			{
				Link = (Shell32.ShellLinkObject)folderItem.GetLink;
				Link.Resolve( 1 );
			}
		}

		#endregion Constructor

		#region Properties

		public string Arguments
		{
			get
			{
				if( !Valid )
				{
					return null;
				}

				return Link.Arguments;
			}
		}

		/// <value>
		///   Gets or sets a description of the shortcut.
		/// </value>
		public string Description
		{
			get
			{
				if( !Valid )
				{
					return null;
				}

				return Link.Description;
			}
		}

		public string IconPath
		{
			get
			{
				if( !Valid )
				{
					return null;
				}

				string location;
				Link.GetIconLocation( out location );

				return location;
			}
		}

		public string Path
		{
			get
			{
				if( !Valid )
				{
					return null;
				}

				return Link.Path;
			}
		}

		public bool Valid
		{
			get
			{
				return Link != null;
			}
		}

		#endregion Properties

		#region Attributes

		private Shell32.ShellLinkObject Link;

		#endregion Attributes
	}
}
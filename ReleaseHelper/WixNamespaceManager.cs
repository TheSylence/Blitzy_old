// $Id$

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace ReleaseHelper
{
	internal class WixNamespaceManager : XmlNamespaceManager
	{
		public const string Namespace = "http://schemas.microsoft.com/wix/2006/wi";
		public const string Prefix = "w";

		public WixNamespaceManager( XmlNameTable nameTable )
			: base( nameTable )
		{
			AddNamespace( Prefix, Namespace );
		}
	}
}
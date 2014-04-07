// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	public class TextInputParameter
	{
		#region Constructor

		public TextInputParameter( string label, string caption, string value = null, object token = null )
		{
			LabelText = label;
			Caption = caption;
			Input = value;
			Token = token;
		}

		#endregion Constructor

		#region Properties

		public readonly string Caption;

		public readonly string LabelText;

		public readonly object Token;

		public string Input;

		#endregion Properties
	}
}
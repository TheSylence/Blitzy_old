﻿// $Id$

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Blitzy.Messages
{
	[ExcludeFromCodeCoverage]
	internal class InputCaretPositionMessage
	{
		public readonly int Index;

		public InputCaretPositionMessage( int index )
		{
			Index = index;
		}
	}
}
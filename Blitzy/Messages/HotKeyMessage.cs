

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Blitzy.Messages
{
	internal class HotKeyMessage
	{
		public readonly Key Key;

		public readonly ModifierKeys Modifiers;

		public HotKeyMessage( ModifierKeys modifiers, Key key )
		{
			Modifiers = modifiers;
			Key = key;
		}
	}
}
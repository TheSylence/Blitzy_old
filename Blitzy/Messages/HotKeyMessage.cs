using System.Windows.Input;

namespace Blitzy.Messages
{
	internal class HotKeyMessage
	{
		public HotKeyMessage( ModifierKeys modifiers, Key key )
		{
			Modifiers = modifiers;
			Key = key;
		}

		public readonly Key Key;

		public readonly ModifierKeys Modifiers;
	}
}
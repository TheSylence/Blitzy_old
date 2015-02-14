using System.Diagnostics.CodeAnalysis;

namespace Blitzy.ViewServices
{
	[ExcludeFromCodeCoverage]
	public class TextInputParameter
	{
		public TextInputParameter( string label, string caption, string value = null, object token = null )
		{
			LabelText = label;
			Caption = caption;
			Input = value;
			Token = token;
		}

		public readonly string Caption;

		public readonly string LabelText;

		public readonly object Token;

		public string Input;
	}
}
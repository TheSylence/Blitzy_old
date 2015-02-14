using System;
using System.Globalization;
using System.Linq;

namespace Blitzy.Model
{
	internal class CommandName
	{
		public CommandName( string name )
		{
			OrigName = name;
			Name = name.ToLowerInvariant();
			Words = Name.Split( SplitChars, StringSplitOptions.RemoveEmptyEntries );
		}

		public bool Match( string input )
		{
			if( Name.Contains( input ) )
				return true;

			bool match = true;

			// Find: Metallica - Nothing else matters
			// Input Metallica nothing
			string[] inputWords = input.Split( SplitChars, StringSplitOptions.RemoveEmptyEntries );
			for( int i = 0; i < Words.Length && i < inputWords.Length; ++i )
			{
				if( !Words[i].Contains( inputWords[i] ) )
				{
					match = false;
					break;
				}
			}

			if( !match )
			{
				match = Words.Intersect( inputWords ).Count() == inputWords.Length;
			}

			// Find: Visual Studio Command Prompt
			// Input: vscp
			if( !match )
			{
				if( input.Length <= Words.Length )
				{
					match = true;
					for( int i = 0; i < input.Length && i <= Words.Length; ++i )
					{
						if( !Words[i].StartsWith( input[i].ToString( CultureInfo.InvariantCulture ), true, CultureInfo.CurrentUICulture ) )
						{
							match = false;
							break;
						}
					}
				}
			}

			return match;
		}

		public string OrigName { get; private set; }

		private static readonly char[] SplitChars = { ' ', '-', '.', ',', ';', '/', '\\' };
		private readonly string Name;
		private readonly string[] Words;
	}
}
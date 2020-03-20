using System.Diagnostics.CodeAnalysis;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class PathObjectParser : IJsonPathParser
	{
		private static readonly string allowedChars = "_'\"*";

		public bool Handles(string input, int index)
		{
			if (index + 1 >= input.Length)
				return false;

			var check = index + 1;

			if (input[index] == '.')
			{
				if (input[index + 1] == '[' && input[index + 2].In('\'', '"'))
					check++;
				return char.IsLetterOrDigit(input[check]) || allowedChars.IndexOf(input[check]) >= 0;
			}

			return input[index] == '[' &&
				   input[index + 1].In('\'', '"') &&
				   (char.IsLetterOrDigit(input[check]) || allowedChars.IndexOf(input[check]) >= 0);
		}

		public bool TryParse(string source, ref int index, [NotNullWhen(true)] ref JsonPath? path, [NotNullWhen(false)] out string? errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}

			if (source[index] == '.')
				index++;

			var isBracketed = false;
			if (source[index] == '[')
			{
				isBracketed = true;
				index++;
			}

		    if (source[index] == '*')
			{
				path = path.Name();
				index++;
				errorMessage = null!;
				return true;
			}

			if (!source.TryGetKey(ref index, out var key, out errorMessage)) return false;

			if (isBracketed)
			{
				if (source[index] != ']') {
					errorMessage = "Expected close bracket.";
					return false;
				}
				index++;
			}

			path = path.Name(key);
			return true;
		}
	}
}
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Parsing
{
	internal class SearchParser : IJsonPathParser
	{
		private const string _allowedChars = "_'\"*";

		public bool Handles(string input, int index)
		{
			if (index + 2 >= input.Length)
				return false;

			var check = index + 2;

			if (input[index] != '.' || input[index + 1] != '.')
				return false;

			if (input[index + 2] == '[' && _allowedChars.IndexOf(input[index + 3]) >= 0)
				check++;
			return char.IsLetterOrDigit(input[check]) || _allowedChars.IndexOf(input[check]) >= 0;

			//return input[index] == '.' &&
			//       input[index + 1] == '.' &&
			//       (char.IsLetterOrDigit(input[index + 2]) || _allowedChars.IndexOf(input[index + 2]) >= 0);
		}

		public bool TryParse(string source, ref int index, [NotNullWhen(true)] ref JsonPath? path, [NotNullWhen(false)] out string? errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}

			index += 2;
			if (source[index] == '*')
			{
				path = path.Search();
				index++;
				errorMessage = null!;
				return true;
			}

			if (!source.TryGetKey(ref index, out var key, out errorMessage)) return false;

			path = path.Search(key);
			return true;
		}
	}
}
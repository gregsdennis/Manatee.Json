using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class SearchParser : IJsonPathParser
	{
		private static readonly string allowedChars = "_'\"*";

		public bool Handles(string input, int index)
		{
			if (index + 2 >= input.Length)
				return false;

			return input[index] == '.' &&
			       input[index + 1] == '.' &&
			       (char.IsLetterOrDigit(input[index + 2]) || allowedChars.IndexOf(input[index + 2]) >= 0);
		}

		public bool TryParse(string source, ref int index, ref JsonPath path, out string errorMessage)
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
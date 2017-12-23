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

		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null) return "Start token not found.";

			index += 2;

			if (source[index] == '*')
			{
				path = path.Search();
				index++;
				return null;
			}

			var error = source.GetKey(ref index, out var key);
			if (error != null) return error;

			path = path.Search(key);
			return null;
		}
	}
}
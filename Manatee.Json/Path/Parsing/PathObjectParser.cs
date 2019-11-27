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

			return input[index] == '.'
				&& (char.IsLetterOrDigit(input[index + 1])
					|| allowedChars.IndexOf(input[index + 1]) >= 0);
		}

		public bool TryParse(string source, ref int index, ref JsonPath path, out string errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}

		    index++;
		    if (source[index] == '*')
			{
				path = path.Name();
				index++;
				errorMessage = null!;
				return true;
			}

			if (!source.TryGetKey(ref index, out var key, out errorMessage)) return false;

			path = path.Name(key);
			return true;
		}
	}
}
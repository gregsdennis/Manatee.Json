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
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

		    index++;

			if (source[index] == '*')
			{
				path = path.Name();
				index++;
				return null;
			}

		    var error = source.GetKey(ref index, out var key);
			if (error != null) return error;

			path = path.Name(key);
			return null;
		}
	}
}
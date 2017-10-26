using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class PathObjectParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.Length >= 2 && input[0] == '.' && (char.IsLetterOrDigit(input[1]) || input[1].In('_', '\'', '"', '*'));
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
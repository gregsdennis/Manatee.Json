using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class SearchParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("..") && input.Length >= 3 && (char.IsLetterOrDigit(input[2]) || input[2].In('_', '\'', '"', '*'));
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

			index += 2;

			if (source[index] == '*')
			{
				path = path.Search();
				index++;
				return null;
			}

			string key;
			var error = source.GetKey(ref index, out key);
			if (error != null) return error;

			path = path.Search(key);
			return null;
		}
	}
}
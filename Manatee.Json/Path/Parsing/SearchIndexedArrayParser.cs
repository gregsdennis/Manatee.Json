using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Manatee.Json.Path.Parsing
{
	internal class SearchIndexedArrayParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 3 >= input.Length)
				return false;

			return input[index] == '.' &&
			       input[index + 1] == '.' &&
			       input[index + 2] == '[' &&
			       (char.IsDigit(input[index + 3]) || input[index + 3] == '-' || input[index + 3] == ':');
		}

		public bool TryParse(string source, ref int index, [NotNullWhen(true)] ref JsonPath? path, [NotNullWhen(false)] out string? errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}

			index += 2;
			if (!source.TryGetSlices(ref index, out var slices, out errorMessage)) return false;

			path = path.SearchArray(slices.ToArray());
			return true;
		}
	}
}

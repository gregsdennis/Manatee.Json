using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Parsing
{
	internal class WildcardArrayParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 2 >= input.Length)
				return false;

			return input[index] == '[' &&
			       input[index + 1] == '*' &&
			       input[index + 2] == ']';
		}

		public bool TryParse(string source, ref int index, [NotNullWhen(true)] ref JsonPath? path, [NotNullWhen(false)] out string? errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}
	
			path = path.Array();
			index += 3;
			errorMessage = null!;
			return true;
		}
	}
}
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

		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			path = path.Array();
			index += 3;
			return null;
		}
	}
}
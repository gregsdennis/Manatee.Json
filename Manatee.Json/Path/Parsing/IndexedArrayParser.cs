using System.Linq;

namespace Manatee.Json.Path.Parsing
{
	internal class IndexedArrayParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 1 >= input.Length) return false;

			return input[index] == '[' &&
			       (char.IsDigit(input[index + 1]) || input[index + 1] == '-' || input[index + 1] == ':');
		}

		public bool TryParse(string source, ref int index, ref JsonPath path, out string errorMessage)
		{
			if (path == null)
			{
				errorMessage = "Start token not found.";
				return false;
			}

			if (!source.TryGetSlices(ref index, out var slices, out errorMessage)) return false;

			path = path.Array(slices.ToArray());
			return true;
		}
	}
}

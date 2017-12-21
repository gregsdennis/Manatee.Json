using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class IndexedArrayParser : IJsonPathParser
	{
		public bool Handles(string input, int index)
		{
			if (index + 1 >= input.Length)
				return false;

			return input[index] == '['
				&& (char.IsDigit(input[index + 1])
					|| (input[index + 1] == '-' || input[index + 1] == ':'));
		}

		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null) return "Start token not found.";

			var error = source.GetSlices(ref index, out IList<Slice> slices);
			if (error != null) return error;

			path = path.Array(slices.ToArray());
			return null;
		}
	}
}

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class IndexedArrayParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.Length > 1 && input[0] == '[' && (char.IsDigit(input[1]) || input[1].In('-', ':'));
			//return input.Length > 1 && input[0] == '[' && (char.IsDigit(input[1]) || input[1].In('"', '\''));
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

			string error;
			Slice lastSlice;
			var slices = new List<Slice>();
			do
			{
				index++;
				error = GetSlice(source, ref index, out lastSlice);
				if (lastSlice != null)
					slices.Add(lastSlice);
			} while (error == null && lastSlice != null);
			if (error != null) return error;

			index++;
			if (!slices.Any())
				return "Index required inside '[]'";
			path = path.Array(slices.ToArray());
			return null;
		}

		private static string GetSlice(string source, ref int index, out Slice slice)
		{
			slice = null;
			if (source[index - 1] == ']') return null;

			int? n1, n2, n3;

			var error = GetNumber(source, ref index, out n1);
			if (error != null) return error;
			if (n1.HasValue && source[index].In(',', ']'))
			{
				slice = new Slice(n1.Value);
				return null;
			}
			if (source[index] != ':')
				return "Expected ':', ',', or ']'.";

			index++;
			error = GetNumber(source, ref index, out n2);
			if (error != null) return error;
			if (source[index].In(',', ']'))
			{
				slice = new Slice(n1, n2);
				return null;
			}
			if (source[index] != ':')
				return "Expected ':', ',', or ']'.";

			index++;
			error = GetNumber(source, ref index, out n3);
			if (error != null) return error;
			if (source[index].In(',', ']'))
			{
				slice = new Slice(n1, n2, n3);
				return null;
			}
			return "Expected ',' or ']'.";
		}

		private static string GetNumber(string source, ref int index, out int? number)
		{
			int value;
			var text = new string(source.Substring(index).TakeWhile(c => char.IsDigit(c) || c == '-').ToArray());
			if (text.Length == 0 && source[index].In(':',',',']'))
			{
				number = null;
				return null;
			}
			if (!int.TryParse(text, out value))
			{
				number = null;
				return "Expected number.";
			}

			index += text.Length;
			number = value;
			return null;
		}
	}
}

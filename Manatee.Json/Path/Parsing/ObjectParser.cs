using System;
using System.IO;

namespace Manatee.Json.Path.Parsing
{
	internal class ObjectParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.Length > 1 && input[0] == '.' && input[1] != '.';
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

		    index++;

			if (source[index] == '*')
			{
				path = path.Wildcard();
				index++;
				return null;
			}

			string key;
			var error = source.GetKey(ref index, out key);
			if (error != null) return error;

			path = path.Name(key);
			return null;
		}
	}
}
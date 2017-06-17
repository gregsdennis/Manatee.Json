using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class ArrayParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '[';
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var array = new JsonArray();
			value = array;
			var length = source.Length;
			index++;
			while (index < length)
			{
				var message = source.SkipWhiteSpace(ref index, length, out char c);
				if (message != null) return message;
				// check for empty array
				if (c == ']')
					if (array.Count == 0)
					{
						index++;
						break;
					}
					else return "Expected value.";
				// get value
				message = JsonParser.Parse(source, ref index, out JsonValue item);
				array.Add(item);
				if (message != null) return message;
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for end or separator
				index++;
				if (c == ']')
				{
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var array = new JsonArray();
			value = array;
			while (!stream.EndOfStream)
			{
				stream.Read(); // waste the '[' or ','
				var message = stream.SkipWhiteSpace(out char c);
				if (message != null) return message;
				// check for empty array
				if (c == ']')
					if (array.Count == 0)
					{
						stream.Read(); // waste the ']'
						break;
					}
					else return "Expected value.";
				// get value
				message = JsonParser.Parse(stream, out JsonValue item);
				array.Add(item);
				if (message != null) return message;
				message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				// check for end or separator
				if (c == ']')
				{
					stream.Read(); // waste the ']'
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
	}
}
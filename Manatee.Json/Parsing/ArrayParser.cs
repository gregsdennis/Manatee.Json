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
				char c;
				var message = source.SkipWhiteSpace(ref index, length, out c);
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
				JsonValue item;
				message = JsonParser.Parse(source, ref index, out item);
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
				char c;
				var message = stream.SkipWhiteSpace(out c);
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
				JsonValue item;
				message = JsonParser.Parse(stream, out item);
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
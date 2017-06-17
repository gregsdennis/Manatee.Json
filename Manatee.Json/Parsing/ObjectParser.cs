using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class ObjectParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '{';
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var obj = new JsonObject();
			value = obj;
			var length = source.Length;
			index++;
			while (index < length)
			{
				var message = source.SkipWhiteSpace(ref index, length, out char c);
				if (message != null) return message;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						index++;
						break;
					}
					else return "Expected key.";
				// get key
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				if (c != '\"') return "Expected key.";
				message = JsonParser.Parse(source, ref index, out JsonValue item);
				if (message != null) return message;
				var key = item.String;
				// check for colon
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				if (c != ':')
				{
					obj.Add(key, null);
					return "Expected ':'.";
				}
				index++;
				// get value (whitespace is removed in Parse)
				message = JsonParser.Parse(source, ref index, out item);
				obj.Add(key, item);
				if (message != null) return message;
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for end or separator
				index++;
				if (c == '}')
				{
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var obj = new JsonObject();
			value = obj;
			while (!stream.EndOfStream)
			{
				stream.Read(); // waste the '{' or ','
				var message = stream.SkipWhiteSpace(out char c);
				if (message != null) return message;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						stream.Read(); // waste the '}'
						break;
					}
					else return "Expected key.";
				// get key
				message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				if (c != '\"') return "Expected key.";
				message = JsonParser.Parse(stream, out JsonValue item);
				if (message != null) return message;
				var key = item.String;
				// check for colon
				message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				if (c != ':')
				{
					obj.Add(key, null);
					return "Expected ':'.";
				}
				stream.Read(); // waste the ':'
				// get value (whitespace is removed in Parse)
				message = JsonParser.Parse(stream, out item);
				obj.Add(key, item);
				if (message != null) return message;
				message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				// check for end or separator
				if (c == '}')
				{
					stream.Read(); // waste the '}'
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
	}
}
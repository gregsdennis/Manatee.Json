using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(StreamReader stream, CancellationToken token)
		{
			var obj = new JsonObject();
			var value = obj;
			while (!stream.EndOfStream)
			{
				if (token.IsCancellationRequested)
					return ("Parsing incomplete. The task was cancelled.", null);
				await stream.TryRead(); // waste the '{' or ','
				var (message, c) = await stream.SkipWhiteSpaceAsync();
				if (message != null) return (message, value);
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						await stream.TryRead(); // waste the '}'
						break;
					}
					else return ("Expected key.", value);
				// get key
				(message, c) = await stream.SkipWhiteSpaceAsync();
				if (message != null) return (message, value);
				if (c != '\"') return ("Expected key.", value);
				JsonValue item;
				(message, item) = await JsonParser.TryParseAsync(stream, token);
				if (message != null) return (message, value);
				var key = item.String;
				// check for colon
				(message, c) = await stream.SkipWhiteSpaceAsync();
				if (message != null) return (message, value);
				if (c != ':')
				{
					obj.Add(key, null);
					return ("Expected ':'.", value);
				}
				await stream.TryRead(); // waste the ':'
				// get value (whitespace is removed in Parse)
				message = JsonParser.Parse(stream, out item);
				obj.Add(key, item);
				if (message != null) return (message, value);
				(message, c) = await stream.SkipWhiteSpaceAsync();
				if (message != null) return (message, value);
				// check for end or separator
				if (c == '}')
				{
					await stream.TryRead(); // waste the '}'
					break;
				}
				if (c != ',') return ("Expected ','.", value);
			}
			return (null, value);
		}
	}
}
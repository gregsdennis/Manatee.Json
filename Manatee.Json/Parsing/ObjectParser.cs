using System;
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

			bool complete = false;

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
						complete = true;
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
					complete = true;
					break;
				}
				if (c != ',') return "Expected ','.";
			}

			if (!complete)
				return "Unterminated object (missing '}').";

			return null;
		}
		public string TryParse(TextReader stream, out JsonValue value)
		{
			bool complete = false;

			var obj = new JsonObject();
			value = obj;
			while (stream.Peek() != -1)
			{
				stream.Read(); // waste the '{' or ','
				var message = stream.SkipWhiteSpace(out char c);
				if (message != null) return message;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						complete = true;
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
					complete = true;
					stream.Read(); // waste the '}'
					break;
				}
				if (c != ',') return "Expected ','.";
			}

			if (!complete)
				return "Unterminated object (missing '}').";

			return null;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			bool complete = false;

			var obj = new JsonObject();

			var scratch = SmallBufferCache.Acquire(1);

			string errorMessage = null;
			while (stream.Peek() != -1)
			{
				if (token.IsCancellationRequested)
				{
					errorMessage = "Parsing incomplete. The task was cancelled.";
					break;
				}
				await stream.TryRead(scratch, 0, 1, token); // waste the '{' or ','
				char c;
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null) break;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						complete = true;
						await stream.TryRead(scratch, 0, 1, token); // waste the '}'
						break;
					}
					else
					{
						errorMessage = "Expected key.";
						break;
					}
				// get key
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null) break; ;
				if (c != '\"')
				{
					errorMessage = "Expected key.";
					break;
				}

				JsonValue item;
				(errorMessage, item) = await JsonParser.TryParseAsync(stream, token);
				if (errorMessage != null) break;
				var key = item.String;
				// check for colon
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null) break;
				if (c != ':')
				{
					obj.Add(key, null);
					errorMessage = "Expected ':'.";
					break;
				}
				await stream.TryRead(scratch, 0, 1, token); // waste the ':'
													 // get value (whitespace is removed in Parse)
				errorMessage = JsonParser.Parse(stream, out item);
				obj.Add(key, item);
				if (errorMessage != null) break;
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null) break;
				// check for end or separator
				if (c == '}')
				{
					complete = true;
					await stream.TryRead(scratch, 0, 1, token); // waste the '}'
					break;
				}
				if (c != ',')
				{
					errorMessage = "Expected ','.";
					break;
				}
			}

			if (!complete && errorMessage == null)
			{
				errorMessage = "Unterminated object (missing '}').";
			}

			SmallBufferCache.Release(scratch);
			return (errorMessage, obj);
		}
	}
}
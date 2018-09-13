using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
			System.Diagnostics.Debug.Assert(index < source.Length && source[index] == '[');

			bool complete = false;
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
						complete = true;
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
					complete = true;
					break;
				}
				if (c != ',') return "Expected ','.";
			}

			if (!complete)
				return "Unterminated array (missing ']')";

			return null;
		}
		public string TryParse(TextReader stream, out JsonValue value)
		{
			System.Diagnostics.Debug.Assert(stream.Peek() == '[');

			bool complete = false;
			var array = new JsonArray();
			value = array;
			while (stream.Peek() != -1)
			{
				stream.Read(); // waste the '[' or ','
				var message = stream.SkipWhiteSpace(out char c);
				if (message != null) return message;
				// check for empty array
				if (c == ']')
					if (array.Count == 0)
					{
						complete = true;
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
					complete = true;
					stream.Read(); // waste the ']'
					break;
				}
				if (c != ',') return "Expected ','.";
			}

			if (!complete)
				return "Unterminated array (missing ']')";

			return null;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			System.Diagnostics.Debug.Assert(stream.Peek() == '[');

			var scratch = SmallBufferCache.Acquire(1);
			var array = new JsonArray();

			bool complete = false;

			char c;
			string errorMessage = null;
			while (stream.Peek() != -1)
			{
				if (token.IsCancellationRequested)
				{
					errorMessage = "Parsing incomplete. The task was cancelled.";
					break;
				}

				await stream.TryRead(scratch, 0, 1, token); // waste the '[' or ','
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null)
					break;

				// check for empty array
				if (c == ']')
				{
					if (array.Count == 0)
					{
						complete = true;
						await stream.TryRead(scratch, 0, 1, token); // waste the ']'
						break;
					}
					else
					{
						errorMessage = "Expected value.";
						break;
					}
				}

				// get value
				JsonValue item;
				(errorMessage, item) = await JsonParser.TryParseAsync(stream, token);
				array.Add(item);
				if (errorMessage != null)
					break;

				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null)
					break;

				// check for end or separator
				if (c == ']')
				{
					complete = true;
					await stream.TryRead(scratch, 0, 1, token); // waste the ']'
					break;
				}

				if (c != ',')
				{
					errorMessage = "Expected ','.";
					break;
				}
			}

			if (!complete)
			{
				errorMessage = "Unterminated array (missing ']')";
			}

			SmallBufferCache.Release(scratch);
			return (errorMessage, array);
		}
	}
}
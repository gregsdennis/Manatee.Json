using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
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
		public bool TryParse(string source, ref int index, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage, bool allowExtraChars)
		{
			Debug.Assert(index < source.Length && source[index] == '[');

			var complete = false;
			var array = new JsonArray();
			value = array;
			var length = source.Length;
			index++;
			while (index < length)
			{
				errorMessage = source.SkipWhiteSpace(ref index, length, out char c);
				if (errorMessage != null) return false;
				// check for empty array
				if (c == ']')
				{
					if (array.Count == 0)
					{
						complete = true;
						index++;
						break;
					}

					errorMessage = "Expected value.";
					return false;
				}
				// get value
				var success = JsonParser.TryParse(source, ref index, out var item, out errorMessage);
				array.Add(item!);
				if (!success) return false;
				errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return false;
				// check for end or separator
				index++;
				if (c == ']')
				{
					complete = true;
					break;
				}

				if (c != ',')
				{
					errorMessage = "Expected ','.";
					return false;
				}
			}

			if (!complete)
			{
				errorMessage = "Unterminated array (missing ']')";
				return false;
			}

			errorMessage = null;
			return true;
		}
		public bool TryParse(TextReader stream, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage)
		{
			Debug.Assert(stream.Peek() == '[');

			bool complete = false;
			var array = new JsonArray();
			value = array;
			while (stream.Peek() != -1)
			{
				stream.Read(); // waste the '[' or ','
				errorMessage = stream.SkipWhiteSpace(out var c);
				if (errorMessage != null) return false;
				// check for empty array
				if (c == ']')
				{
					if (array.Count == 0)
					{
						complete = true;
						stream.Read(); // waste the ']'
						break;
					}

					errorMessage = "Expected value.";
					return false;
				}
				// get value
				var success = JsonParser.TryParse(stream, out var item, out errorMessage);
				array.Add(item!);
				if (!success) return false;
				errorMessage = stream.SkipWhiteSpace(out c);
				if (errorMessage != null) return false;
				// check for end or separator
				if (c == ']')
				{
					complete = true;
					stream.Read(); // waste the ']'
					break;
				}

				if (c != ',')
				{
					errorMessage = "Expected ','.";
					return false;
				}
			}

			if (!complete)
			{
				errorMessage = "Unterminated array (missing ']')";
				return false;
			}

			errorMessage = null;
			return true;
		}
		public async Task<(string? errorMessage, JsonValue? value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			Debug.Assert(stream.Peek() == '[');

			var scratch = SmallBufferCache.Acquire(1);
			var array = new JsonArray();

			bool complete = false;

			string? errorMessage = null;
			while (stream.Peek() != -1)
			{
				if (token.IsCancellationRequested)
				{
					errorMessage = "Parsing incomplete. The task was cancelled.";
					break;
				}

				await stream.TryRead(scratch, 0, 1, token); // waste the '[' or ','
				char c;
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

					errorMessage = "Expected value.";
					break;
				}

				// get value
				JsonValue? item;
				(errorMessage, item) = await JsonParser.TryParseAsync(stream, token);
				array.Add(item!);
				if (errorMessage != null) break;

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
				errorMessage ??= "Unterminated array (missing ']')";

			SmallBufferCache.Release(scratch);
			return (errorMessage, array);
		}
	}
}
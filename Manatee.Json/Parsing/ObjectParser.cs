using System.Diagnostics.CodeAnalysis;
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
		public bool TryParse(string source, ref int index, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage, bool allowExtraChars)
		{
			bool complete = false;

			var obj = new JsonObject();
			value = obj;
			var length = source.Length;
			index++;
			while (index < length)
			{
				errorMessage = source.SkipWhiteSpace(ref index, length, out char c);
				if (errorMessage != null) return false;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						complete = true;
						index++;
						break;
					}
					else
					{
						errorMessage = "Expected key.";
						return false;
					}
				// get key
				errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return false;
				if (c != '\"')
				{
					errorMessage = "Expected key.";
					return false;
				}
				if (!JsonParser.TryParse(source, ref index, out var item, out errorMessage)) return false;
				var key = item!.String;
				// check for colon
				errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return false;
				if (c != ':')
				{
					obj.Add(key, null!);
					errorMessage = "Expected ':'.";
					return false;
				}
				index++;
				// get value (whitespace is removed in Parse)
				var success = JsonParser.TryParse(source, ref index, out item, out errorMessage);
				obj.Add(key, item!);
				if (!success) return false;
				errorMessage = source.SkipWhiteSpace(ref index, length, out c);
				if (errorMessage != null) return false;
				// check for end or separator
				index++;
				if (c == '}')
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
				errorMessage = "Unterminated object (missing '}').";
				return false;
			}

			errorMessage = null;
			return true;
		}
		public bool TryParse(TextReader stream, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage)
		{
			bool complete = false;

			var obj = new JsonObject();
			value = obj;
			while (stream.Peek() != -1)
			{
				stream.Read(); // waste the '{' or ','
				errorMessage = stream.SkipWhiteSpace(out char c);
				if (errorMessage != null) return false;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0)
					{
						complete = true;
						stream.Read(); // waste the '}'
						break;
					}
					else
					{
						errorMessage = "Expected key.";
						return false;
					}
				// get key
				errorMessage = stream.SkipWhiteSpace(out c);
				if (errorMessage != null) return false;
				if (c != '\"')
				{
					errorMessage = "Expected key.";
					return false;
				}
				if (!JsonParser.TryParse(stream, out var item, out errorMessage)) return false;
				var key = item!.String;
				// check for colon
				errorMessage = stream.SkipWhiteSpace(out c);
				if (errorMessage != null) return false;
				if (c != ':')
				{
					obj.Add(key, null!);
					errorMessage = "Expected ':'.";
					return false;
				}
				stream.Read(); // waste the ':'
							   // get value (whitespace is removed in Parse)
				var success = JsonParser.TryParse(stream, out item, out errorMessage);
				obj.Add(key, item!);
				if (!success) return false;
				errorMessage = stream.SkipWhiteSpace(out c);
				if (errorMessage != null) return false;
				// check for end or separator
				if (c == '}')
				{
					complete = true;
					stream.Read(); // waste the '}'
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
				errorMessage = "Unterminated object (missing '}').";
				return false;
			}

			errorMessage = null;
			return true;
		}
		public async Task<(string? errorMessage, JsonValue? value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			bool complete = false;

			var obj = new JsonObject();

			var scratch = SmallBufferCache.Acquire(1);

			string? errorMessage = null;
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
				if (errorMessage != null) break;
				if (c != '\"')
				{
					errorMessage = "Expected key.";
					break;
				}

				JsonValue? item;
				(errorMessage, item) = await JsonParser.TryParseAsync(stream, token);
				if (errorMessage != null) break;
				var key = item!.String;
				// check for colon
				(errorMessage, c) = await stream.SkipWhiteSpaceAsync(scratch);
				if (errorMessage != null) break;
				if (c != ':')
				{
					obj.Add(key, null!);
					errorMessage = "Expected ':'.";
					break;
				}
				await stream.TryRead(scratch, 0, 1, token); // waste the ':'
															// get value (whitespace is removed in Parse)
				(errorMessage, item) = await JsonParser.TryParseAsync(stream, token);
				obj.Add(key, item!);
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
				errorMessage = "Unterminated object (missing '}').";

			SmallBufferCache.Release(scratch);
			return (errorMessage, obj);
		}
	}
}
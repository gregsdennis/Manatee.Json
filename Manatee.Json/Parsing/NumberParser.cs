using System;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class NumberParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '0' || c == '1' || c == '2' || c == '3' || c == '4' ||
			       c == '5' || c == '6' || c == '7' || c == '8' || c == '9' ||
			       c == '-';
		}

		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			if (source == null)
				throw new ArgumentNullException(nameof(source));
			if (index >= source.Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			value = null;

			var originalIndex = index;
			while (index < source.Length)
			{
				var c = source[index];
				if (char.IsWhiteSpace(c) || c == ',' || c == ']' || c == '}') break;

				var isNumber = _IsNumberChar(c);
				if (!isNumber && allowExtraChars) break;
				if (!isNumber)
				{
					return "Expected ',', ']', or '}'.";
				}

				index++;
			}

			var result = source.Substring(originalIndex, index - originalIndex);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
			{
				return $"Value not recognized: '{result}'";
			}

			value = dbl;
			return null;
		}

		public string TryParse(TextReader stream, out JsonValue value)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			var buffer = StringBuilderCache.Acquire();
			var bufferIndex = 0;
			while (stream.Peek() != -1)
			{
				var c = (char)stream.Peek();
				if (char.IsWhiteSpace(c) || c == ',' || c == ']' || c == '}') break;

				stream.Read(); // eat the character

				if (!_IsNumberChar(c))
				{
					value = null;
					StringBuilderCache.Release(buffer);
					return "Expected ',', ']', or '}'.";
				}

				buffer.Append(c);
				bufferIndex++;
			}

			var result = StringBuilderCache.GetStringAndRelease(buffer);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
			{
				value = null;
				return $"Value not recognized: '{result}'";
			}

			value = dbl;
			return null;
		}

		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			if (stream == null)
				throw new ArgumentNullException(nameof(stream));

			var buffer = StringBuilderCache.Acquire();
			var scratch = SmallBufferCache.Acquire(1);

			string errorMessage = null;
			while (stream.Peek() != -1)
			{
				if (token.IsCancellationRequested)
				{
					errorMessage = "Parsing incomplete. The task was cancelled.";
					break;
				}

				var c = (char)stream.Peek();
				if (char.IsWhiteSpace(c) || c == ',' || c == ']' || c == '}') break;

				await stream.TryRead(scratch, 0, 1, token); // eat the character

				if (!_IsNumberChar(c))
				{
					errorMessage = "Expected ',', ']', or '}'.";
					break;
				}

				buffer.Append(c);
			}

			SmallBufferCache.Release(scratch);

			if (errorMessage != null)
			{
				StringBuilderCache.Release(buffer);
				return (errorMessage, null);
			}

			var result = StringBuilderCache.GetStringAndRelease(buffer);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
				return ($"Value not recognized: '{result}'", null);

			return (null, dbl);
		}

		private static bool _IsNumberChar(char c)
		{
			return c == '0' || c == '1' || c == '2' || c == '3' || c == '4' ||
			       c == '5' || c == '6' || c == '7' || c == '8' || c == '9' ||
			       c == '-' || c == '+' || c == '.' || c == 'e' || c == 'E';
		}
	}
}

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class NumberParser : IJsonParser
	{
		public bool Handles(char c)
		{
			switch (c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
					return true;
			}
			return false;
		}

		private static bool IsNumberChar(char c)
		{
			switch (c)
			{
				case '0':
				case '1':
				case '2':
				case '3':
				case '4':
				case '5':
				case '6':
				case '7':
				case '8':
				case '9':
				case '-':
				case '+':
				case '.':
				case 'e':
				case 'E':
					return true;
			}
			return false;
		}

		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var sourceLength = source.Length;
			var originalIndex = index;
			while (index < source.Length)
			{
				var c = source[index];
				if (char.IsWhiteSpace(c) || (c == ',' || c == ']' || c == '}')) break;

				var isNumber = IsNumberChar(c);
				if (!isNumber && allowExtraChars) break;
				if (!isNumber)
				{
					value = null;
					return "Expected ',', ']', or '}'.";
				}

				index++;
			}

			var result = source.Substring(originalIndex, index - originalIndex);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
			{
				value = null;
				return $"Value not recognized: '{result}'";
			}

			value = dbl;
			return null;
		}

		public string TryParse(TextReader stream, out JsonValue value)
		{
			var buffer = StringBuilderCache.Acquire();
			var bufferIndex = 0;
			while (stream.Peek() != -1)
			{
				var c = (char)stream.Peek();
				if (char.IsWhiteSpace(c) || (c == ',' || c == ']' || c == '}')) break;

				stream.Read(); // eat the character

				if (!IsNumberChar(c))
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
				if (char.IsWhiteSpace(c) || (c == ',' || c == ']' || c == '}')) break;

				await stream.TryRead(scratch, 0, 1); // eat the character

				if (!IsNumberChar(c))
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
			{
				return ($"Value not recognized: '{result}'", null);
			}

			return (null, dbl);
		}
	}
}

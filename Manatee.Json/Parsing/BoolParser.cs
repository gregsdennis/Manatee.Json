using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class BoolParser : IJsonParser
	{
		private const string _unexpectedEndOfInput = "Unexpected end of input.";

		public bool Handles(char c)
		{
			return c == 't' || c == 'T' || c == 'f' || c == 'F';
		}

		public bool TryParse(string source, ref int index, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage, bool allowExtraChars)
		{
			if (index >= source.Length)
				throw new ArgumentOutOfRangeException(nameof(index));

			value = null;

			if (source[index] == 't' || source[index] == 'T')
			{
				if (index + 4 > source.Length)
				{
					errorMessage = _unexpectedEndOfInput;
					return false;
				}

				if (source.IndexOf("true", index, 4, StringComparison.OrdinalIgnoreCase) != index)
				{
					errorMessage = $"Value not recognized: '{source.Substring(index, 4)}'.";
					return false;
				}

				index += 4;
				value = true;
			}
			else
			{
				if (index + 5 > source.Length)
				{
					errorMessage = _unexpectedEndOfInput;
					return false;
				}

				if (source.IndexOf("false", index, 5, StringComparison.OrdinalIgnoreCase) != index)
				{
					errorMessage = $"Value not recognized: '{source.Substring(index, 5)}'.";
					return false;
				}

				index += 5;
				value = false;
			}

			errorMessage = null;
			return true;
		}

		public bool TryParse(TextReader stream, [NotNullWhen(true)] out JsonValue? value, [NotNullWhen(false)] out string? errorMessage)
		{
			value = null;

			int count;
			var current = (char) stream.Peek();
			if (current == 't' || current == 'T')
				count = 4;
			else
				count = 5;

			var buffer = SmallBufferCache.Acquire(count);
			var charsRead = stream.ReadBlock(buffer, 0, count);
			if (charsRead != count)
			{
				SmallBufferCache.Release(buffer);
				errorMessage = _unexpectedEndOfInput;
				return false;
			}

			if (count == 4)
			{
				if ((buffer[0] == 't' || buffer[0] == 'T')
				    && (buffer[1] == 'r' || buffer[1] == 'R')
				    && (buffer[2] == 'u' || buffer[2] == 'U')
				    && (buffer[3] == 'e' || buffer[3] == 'E'))
					value = true;
				else
				{
					errorMessage = $"Value not recognized: '{new string(buffer, 0, count)}'.";
					return false;
				}
			}
			else
			{
				if ((buffer[0] == 'f' || buffer[0] == 'F') &&
				    (buffer[1] == 'a' || buffer[1] == 'A') &&
				    (buffer[2] == 'l' || buffer[2] == 'L') &&
				    (buffer[3] == 's' || buffer[3] == 'S') &&
				    (buffer[4] == 'e' || buffer[4] == 'E'))
					value = false;
				else
				{
					errorMessage = $"Value not recognized: '{new string(buffer, 0, count)}'.";
					return false;
				}
			}

			SmallBufferCache.Release(buffer);
			errorMessage = null;
			return true;
		}

		public async Task<(string? errorMessage, JsonValue? value)> TryParseAsync(TextReader stream, CancellationToken token)
		{
			var buffer = SmallBufferCache.Acquire(5);
			var count = await stream.ReadAsync(buffer, 0, 4);
			if (count < 4)
			{
				SmallBufferCache.Release(buffer);
				return (_unexpectedEndOfInput, null);
			}

			if (token.IsCancellationRequested)
			{
				SmallBufferCache.Release(buffer);
				return ("Parsing incomplete. The task was cancelled.", null);
			}

			JsonValue? value = null;
			string? errorMessage = null;
			if ((buffer[0] == 't' || buffer[0] == 'T') &&
			    (buffer[1] == 'r' || buffer[1] == 'R') &&
			    (buffer[2] == 'u' || buffer[2] == 'U') &&
			    (buffer[3] == 'e' || buffer[3] == 'E'))
			{
				value = true;
			}
			else if ((buffer[0] == 'f' || buffer[0] == 'F') &&
			         (buffer[1] == 'a' || buffer[1] == 'A') &&
			         (buffer[2] == 'l' || buffer[2] == 'L') &&
			         (buffer[3] == 's' || buffer[3] == 'S'))
			{
				if (await stream.TryRead(buffer, 4, 1, token))
				{
					if (buffer[4] == 'e' || buffer[4] == 'E')
						value = false;
					else
						errorMessage = $"Value not recognized: 'fals{buffer[4]}'.";
				}
				else
					errorMessage = "Unexpected end of input.";
			}
			else
				errorMessage = $"Value not recognized: '{new string(buffer, 0, count)}'.";

			SmallBufferCache.Release(buffer);
			return (errorMessage, value);
		}
	}
}
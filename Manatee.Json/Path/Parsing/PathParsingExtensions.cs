using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal static class PathParsingExtensions
	{
		private static readonly int[] FibSequence = { 8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657 };

		public static string GetKey(this string source, ref int index, out string key)
		{
			return source[index].In('"', '\'')
					   ? GetQuotedKey(source, ref index, out key)
					   : GetBasicKey(source, ref index, out key);
		}

		private static string GetBasicKey(string source, ref int index, out string key)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
			var complete = false;
			var c = (char)0;
			while (index < sourceLength)
			{
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength * 2);
					buffer = newBuffer;
				}
				c = source[index];
				index++;
				if (!char.IsLetterOrDigit(c))
				{
					complete = true;
					index--;
					break;
				}
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			if (!complete && index + 1 < sourceLength)
			{
				key = null;
				return $"The character '{c}' is not supported for unquoted names.";
			}
			var result = new string(buffer, 0, bufferIndex);
			key = result;
			return null;
		}

		private static string GetQuotedKey(string source, ref int index, out string key)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
			var foundEscape = false;
			var complete = false;
			var quoteChar = source[index];
			index++;
			while (index < sourceLength)
			{
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength * 2);
					buffer = newBuffer;
				}
				var c = source[index];
				index++;
				if (c == quoteChar && !foundEscape)
				{
					complete = true;
					break;
				}
				foundEscape = c == '\\';
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			if (!complete)
			{
				key = null;
				return "Could not find end of string value.";
			}
			var result = new string(buffer, 0, bufferIndex);
			string escaped;
			var errorMessage = result.EvaluateEscapeSequences(out escaped);
			key = escaped;
			return errorMessage;
		}
	}
}

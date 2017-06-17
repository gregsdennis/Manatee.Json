using System;
using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class StringParser : IJsonParser
	{
		private static readonly int[] FibSequence = {8, 13, 21, 34, 55, 89, 144, 233, 377, 610, 987, 1597, 2584, 4181, 6765, 10946, 17711, 28657};

		public bool Handles(char c)
		{
			return c == '\"';
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
			var foundEscape = false;
			var complete = false;
			index++;
			while (index < sourceLength)
			{
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength*2);
					buffer = newBuffer;
				}
				var c = source[index];
				index++;
				if (c == '"' && !foundEscape)
				{
					complete = true;
					break;
				}
				foundEscape = !foundEscape && c == '\\';
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			if (!complete)
			{
				value = null;
				return "Could not find end of string value.";
			}
			var result = new string(buffer, 0, bufferIndex);
			var errorMessage = result.EvaluateEscapeSequences(out string escaped);
			value = escaped;
			return errorMessage;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			stream.Read(); // waste the '"'
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var foundEscape = false;
			var complete = false;
			while (!stream.EndOfStream)
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
				var c = (char) stream.Read();
				if (c == '"' && !foundEscape)
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
				value = null;
				return "Could not find end of string value.";
			}
			var result = new string(buffer, 0, bufferIndex);
			var errorMessage = result.EvaluateEscapeSequences(out string escaped);
			value = escaped;
			return errorMessage;
		}
	}
}
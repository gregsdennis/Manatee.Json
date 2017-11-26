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
		private static readonly int[] FibSequence = {8, 13, 21, 34, 55, 89, 144};
		private static readonly char[] NumberChars = "0123456798-+.eE".ToCharArray();

		public bool Handles(char c)
		{
			return c.In('0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '0', '-');
		}
		public string TryParse(string source, ref int index, out JsonValue value, bool allowExtraChars)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			var sourceLength = source.Length;
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
				if (char.IsWhiteSpace(c) || c.In(',', ']', '}')) break;
				var isNumber = NumberChars.Contains(c);
				if (!isNumber && allowExtraChars) break;
				if (!isNumber)
				{
					value = null;
					return "Expected \',\', \']\', or \'}\'.";
				}
				buffer[bufferIndex] = c;
				index++;
				bufferIndex++;
			}
			var result = new string(buffer, 0, bufferIndex);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
			{
				value = null;
				return $"Value not recognized: '{result}'";
			}
			value = dbl;
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
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
				var c = (char) stream.Peek();
				if (char.IsWhiteSpace(c) || c.In(',', ']', '}')) break;
				stream.Read();
				if (!NumberChars.Contains(c))
				{
					value = null;
					return "Expected \',\', \']\', or \'}\'.";
				}
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			var result = new string(buffer, 0, bufferIndex);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
			{
				value = null;
				return $"Value not recognized: '{result}'";
			}
			value = dbl;
			return null;
		}
		public async Task<(string errorMessage, JsonValue value)> TryParseAsync(StreamReader stream, CancellationToken token)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			var bufferIndex = 0;
			while (!stream.EndOfStream)
			{
				if (token.IsCancellationRequested)
					return ("Parsing incomplete. The task was cancelled.", null);
				if (bufferIndex == bufferLength)
				{
					var currentLength = bufferLength;
					bufferSize++;
					bufferLength = FibSequence[bufferSize];
					var newBuffer = new char[bufferLength];
					Buffer.BlockCopy(buffer, 0, newBuffer, 0, currentLength * 2);
					buffer = newBuffer;
				}
				var c = (char)stream.Peek();
				if (char.IsWhiteSpace(c) || c.In(',', ']', '}')) break;
				await stream.TryRead();
				if (!NumberChars.Contains(c))
					return ("Expected \',\', \']\', or \'}\'.", null);
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			var result = new string(buffer, 0, bufferIndex);
			if (!double.TryParse(result, NumberStyles.Any, CultureInfo.InvariantCulture, out double dbl))
				return ($"Value not recognized: '{result}'", null);
			return (null, dbl);
		}
	}
}
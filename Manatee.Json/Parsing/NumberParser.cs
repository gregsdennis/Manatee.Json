/***************************************************************************************

	Copyright 2015 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		NumberParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		NumberParser
	Purpose:		Parses JSON numbers.

***************************************************************************************/
using System;
using System.IO;
using System.Linq;
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
		public string TryParse(string source, ref int index, out JsonValue value)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			int bufferIndex = 0;
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
				if (!NumberChars.Contains(c))
				{
					value = null;
					return "Expected \',\', \']\', or \'}\'.";
				}
				buffer[bufferIndex] = c;
				index++;
				bufferIndex++;
			}
			double dbl;
			var result = new string(buffer, 0, bufferIndex);
			if (!double.TryParse(result, out dbl))
			{
				value = null;
				return string.Format("Value not recognized: '{0}'", result);
			}
			value = dbl;
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			int bufferIndex = 0;
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
				if (char.IsWhiteSpace(c) || c.In(',', ']', '}')) break;
				if (!NumberChars.Contains(c))
				{
					value = null;
					return "Expected \',\', \']\', or \'}\'.";
				}
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			double dbl;
			var result = new string(buffer, 0, bufferIndex);
			if (!double.TryParse(result, out dbl))
			{
				value = null;
				return string.Format("Value not recognized: '{0}'", result);
			}
			value = dbl;
			return null;
		}
	}
}
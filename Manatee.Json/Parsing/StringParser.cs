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
 
	File Name:		StringParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		StringParser
	Purpose:		Parses JSON strings.

***************************************************************************************/
using System;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class StringParser : IJsonParser
	{
		private static readonly int[] FibSequence = {8, 13, 21, 34, 55, 89, 144, 233, 377, 610};

		public bool Handles(char c)
		{
			return c == '\"';
		}
		public JsonValue Parse(string source, ref int index)
		{
			var bufferSize = 0;
			var bufferLength = FibSequence[bufferSize];
			var buffer = new char[bufferLength];
			int bufferIndex = 0;
			var sourceLength = source.Length;
			var foundEscape = false;
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
				if (c == '"' && !foundEscape) break;
				foundEscape = c == '\\';
				buffer[bufferIndex] = c;
				bufferIndex++;
			}
			var result = new string(buffer, 0, bufferIndex);
			return result.EvaluateEscapeSequences();
		}
	}
}
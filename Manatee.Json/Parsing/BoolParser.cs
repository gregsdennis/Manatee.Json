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
 
	File Name:		BoolParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		BoolParser
	Purpose:		Parses JSON boolean values.

***************************************************************************************/

using System;
using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class BoolParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c.In('t', 'T', 'f', 'F');
		}
		public string TryParse(string source, ref int index, out JsonValue value)
		{
			char[] buffer;
			int count;
			if (source[index].In('t', 'T'))
			{
				buffer = new char[4];
				count = 4;
			}
			else
			{
				buffer = new char[5];
				count = 5;
			}
			count = Math.Min(count, source.Length - index);
			for (int i = 0; i < count; i++)
			{
				buffer[i] = source[index + i];
			}
			var result = new string(buffer).ToLower();
			if (result == "true")
			{
				index += 4;
				value = true;
				return null;
			}
			if (result == "false")
			{
				index += 5;
				value = false;
				return null;
			}
			value = null;
			return $"Value not recognized: '{result}'";
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			char[] buffer;
			int count;
			var current = (char) stream.Read();
			if (current.In('t', 'T'))
			{
				buffer = new char[4];
				count = 4;
			}
			else
			{
				buffer = new char[5];
				count = 5;
			}
			buffer[0] = current;
			for (int i = 1; i < count && !stream.EndOfStream; i++)
			{
				buffer[i] = (char) stream.Read();
			}
			if (buffer[count - 1] == (char) 0 && stream.EndOfStream)
			{
				value = null;
				return "Unexpected end of input.";
			}
			var result = new string(buffer).ToLower();
			if (result == "true")
			{
				value = true;
				return null;
			}
			if (result == "false")
			{
				value = false;
				return null;
			}
			value = null;
			return $"Value not recognized: '{result}'";
		}
	}
}
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
 
	File Name:		ArrayParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		ArrayParser
	Purpose:		Parses JSON arrays.

***************************************************************************************/

using System.IO;
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class ArrayParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '[';
		}
		public string TryParse(string source, ref int index, out JsonValue value)
		{
			var array = new JsonArray();
			value = array;
			var length = source.Length;
			index++;
			while (index < length)
			{
				char c;
				var message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for empty array
				if (c == ']')
					if (array.Count == 0)
					{
						index++;
						break;
					}
					else return "Expected value.";
				// get value
				JsonValue item;
				message = JsonParser.Parse(source, ref index, out item);
				array.Add(item);
				if (message != null) return message;
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for end or separator
				index++;
				if (c == ']')
				{
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
		public string TryParse(StreamReader stream, out JsonValue value)
		{
			var array = new JsonArray();
			value = array;
			while (!stream.EndOfStream)
			{
				stream.Read(); // waste the '[' or ','
				char c;
				var message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				// check for empty array
				if (c == ']')
					if (array.Count == 0)
					{
						stream.Read(); // waste the ']'
						break;
					}
					else return "Expected value.";
				// get value
				JsonValue item;
				message = JsonParser.Parse(stream, out item);
				array.Add(item);
				if (message != null) return message;
				message = stream.SkipWhiteSpace(out c);
				if (message != null) return message;
				// check for end or separator
				if (c == ']')
				{
					stream.Read(); // waste the ']'
					break;
				}
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
	}
}
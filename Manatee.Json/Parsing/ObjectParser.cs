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
 
	File Name:		ObjectParser.cs
	Namespace:		Manatee.Json.Parsing
	Class Name:		ObjectParser
	Purpose:		Parses JSON objects.

***************************************************************************************/
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class ObjectParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c == '{';
		}
		public string TryParse(string source, ref int index, out JsonValue value)
		{
			var obj = new JsonObject();
			value = obj;
			var length = source.Length;
			index++;
			while (index < length)
			{
				char c;
				var message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for empty object
				if (c == '}')
					if (obj.Count == 0) break;
					else return "Expected key.";
				// get key
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				if (c != '\"') return "Expected key.";
				JsonValue item;
				message = JsonParser.Parse(source, ref index, out item);
				if (message != null) return message;
				var key = item.String;
				// check for colon
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				if (c != ':')
				{
					obj.Add(key, null);
					return "Expected ':'.";
				}
				index++;
				// get value (whitespace is removed in Parse)
				message = JsonParser.Parse(source, ref index, out item);
				obj.Add(key, item);
				if (message != null) return message;
				message = source.SkipWhiteSpace(ref index, length, out c);
				if (message != null) return message;
				// check for end or separator
				index++;
				if (c == '}') break;
				if (c != ',') return "Expected ','.";
			}
			return null;
		}
	}
}
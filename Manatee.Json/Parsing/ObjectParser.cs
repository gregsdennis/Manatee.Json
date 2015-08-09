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
		public JsonValue Parse(string source, ref int index)
		{
			var obj = new JsonObject();
			var length = source.Length;
			index++;
			while (index < length)
			{
				// allow whitespace
				var c = source.SkipWhiteSpace(ref index, length);
				// check for empty object
				if (c == '}')
					if (obj.Count == 0) break;
					else throw new JsonSyntaxException("Expected key");
				// get key
				c = source.SkipWhiteSpace(ref index, length);
				if (c != '\"')
					throw new JsonSyntaxException("Expected key");
				var key = JsonParser.Parse(source, ref index).String;
				// check for colon
				// allow whitespace
				c = source.SkipWhiteSpace(ref index, length);
				if (c != ':')
					throw new JsonSyntaxException("Expected ','");
				index++;
				// get value (whitespace is removed in Parse)
				obj.Add(key, JsonParser.Parse(source, ref index));
				c = source.SkipWhiteSpace(ref index, length);
				// check for end or separator
				index++;
				if (c == '}') break;
				if (c != ',')
					throw new JsonSyntaxException("Expected ','");
			}
			return obj;
		}
	}
}
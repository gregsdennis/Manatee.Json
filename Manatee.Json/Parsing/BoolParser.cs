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
using Manatee.Json.Internal;

namespace Manatee.Json.Parsing
{
	internal class BoolParser : IJsonParser
	{
		public bool Handles(char c)
		{
			return c.In('t', 'T', 'f', 'F');
		}
		public JsonValue Parse(string source, ref int index)
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
				return true;
			}
			if (result == "false")
			{
				index += 5;
				return false;
			}
			throw new JsonSyntaxException("Unrecognized token");
		}
	}
}
﻿/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		PathObjectParser.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		PathObjectParser
	Purpose:		Parses JSON Path object components.

***************************************************************************************/
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class PathObjectParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.Length >= 2 && input[0] == '.' && (char.IsLetterOrDigit(input[1]) || input[1].In('_', '\'', '"', '*'));
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

		    index++;

			if (source[index] == '*')
			{
				path = path.Wildcard();
				index++;
				return null;
			}

			string key;
			var error = source.GetKey(ref index, out key);
			if (error != null) return error;

			path = path.Name(key);
			return null;
		}
	}
}
/***************************************************************************************

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
 
	File Name:		SearchParser.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		SearchParser
	Purpose:		Parses JSON Path named and wildcard search components.

***************************************************************************************/
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class SearchParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.StartsWith("..") && input.Length > 3 && (char.IsLetterOrDigit(input[2]) || input[2].In('_', '\'', '"'));
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

			index += 2;

			if (source[index] == '*')
			{
				path = path.Search();
				index++;
				return null;
			}

			string key;
			var error = source.GetKey(ref index, out key);
			if (error != null) return error;

			path = path.Search(key);
			return null;
		}
	}
}
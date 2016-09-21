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
 
	File Name:		IndexedArrayParser.cs
	Namespace:		Manatee.Json.Path.Parsing
	Class Name:		IndexedArrayParser
	Purpose:		Parses JSON Path array components with discrete indices.

***************************************************************************************/
using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Parsing
{
	internal class IndexedArrayParser : IJsonPathParser
	{
		public bool Handles(string input)
		{
			return input.Length > 1 && input[0] == '[' && (char.IsDigit(input[1]) || input[1].In('-', ':'));
		}
		public string TryParse(string source, ref int index, ref JsonPath path)
		{
			if (path == null)
				return "Start token not found.";

			IList<Slice> slices;
			var error = source.GetSlices(ref index, out slices);
			if (error != null) return error;

			path = path.Array(slices.ToArray());
			return null;
		}
	}
}

/***************************************************************************************

	Copyright 2014 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		IndexQuery.cs
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		IndexQuery
	Purpose:		Provides indexed array queries.

***************************************************************************************/

using System.Collections.Generic;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Queries
{
	internal class IndexQuery : IJsonPathQuery
	{
		private readonly bool _isForSearch;
		private readonly IEnumerable<int> _indices;

		public IndexQuery(bool isForSearch, params int[] indices)
		{
			_isForSearch = isForSearch;
			_indices = indices;
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json)
		{
			var index = 0;
			var items = json.ToList();
			while (index < items.Count)
			{
				if (_indices.Contains(index))
					yield return items[index];
				index++;
			}
		}
		public override string ToString()
		{
			return _isForSearch ? string.Format("[{0}]", _indices.Join(",")) : _indices.Join(",");
		}
	}
}
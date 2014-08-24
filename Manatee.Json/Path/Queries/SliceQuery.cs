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
 
	File Name:		SliceQuery.cs
	Namespace:		Manatee.Json.Path.Queries
	Class Name:		SliceQuery
	Purpose:		Provides array-slice-syntax queries for arrays.

***************************************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.Queries
{
	internal class SliceQuery : IJsonPathQuery
	{
		private readonly bool _isForSearch;

		public int? Start { get; private set; }
		public int? End { get; private set; }
		public int? Step { get; private set; }

		public SliceQuery(bool isForSearch, int? start, int? end, int? step = null)
		{
			_isForSearch = isForSearch;
			Start = start;
			End = end;
			Step = step;
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json)
		{
			var items = json.ToList();
			var start = ResolveIndex(Start ?? 0, items.Count);
			var end = ResolveIndex(End ?? items.Count, items.Count);
			var step = Math.Max(Step ?? 1, 1);

			var index = start;
			while (index < end)
			{
				yield return items[index];
				index += step;
			}
		}
		public override string ToString()
		{
			var slice = Step.HasValue
				       ? string.Format("{0}:{1}:{2}",
				                       Start.HasValue ? Start.ToString() : string.Empty,
				                       End.HasValue ? End.ToString() : string.Empty,
				                       Step)
				       : string.Format("{0}:{1}",
				                       Start.HasValue ? Start.ToString() : string.Empty,
				                       End.HasValue ? End.ToString() : string.Empty);
			return _isForSearch ? string.Format("[{0}]", slice) : slice;
		}

		private static int ResolveIndex(int index, int count)
		{
			return index < 0 ? count + index : index;
		}
	}
}
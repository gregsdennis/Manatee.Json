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
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		SliceQuery
	Purpose:		Provides array-slice-syntax queries for arrays.

***************************************************************************************/
using System;
using System.Collections.Generic;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class SliceQuery : IJsonPathArrayQuery
	{
		public int? Start { get; private set; }
		public int? End { get; private set; }
		public int? Step { get; private set; }

		public SliceQuery(int? start, int? end, int? step = null)
		{
			Start = start;
			End = end;
			Step = step;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			var start = ResolveIndex(Start ?? 0, json.Count);
			var end = ResolveIndex(End ?? json.Count, json.Count);
			var step = Math.Max(Step ?? 1, 1);

			var index = start;
			while (index < end)
			{
				yield return json[index];
				index += step;
			}
		}
		public override string ToString()
		{
			return Step.HasValue
				       ? string.Format("{0}:{1}:{2}",
				                       Start.HasValue ? Start.ToString() : string.Empty,
				                       End.HasValue ? End.ToString() : string.Empty,
				                       Step)
				       : string.Format("{0}:{1}",
				                       Start.HasValue ? Start.ToString() : string.Empty,
				                       End.HasValue ? End.ToString() : string.Empty);

		}

		private static int ResolveIndex(int index, int count)
		{
			return index < 0 ? count + index : index;
		}
	}
}
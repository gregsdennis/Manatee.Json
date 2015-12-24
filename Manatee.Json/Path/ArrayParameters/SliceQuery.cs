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
		private int? _start;
		private int? _end;
		private int? _step;

		public SliceQuery(int? start, int? end, int? step = null)
		{
			_start = start;
			_end = end;
			_step = step;
		}

		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			var start = ResolveIndex(_start ?? 0, json.Count);
			var end = ResolveIndex(_end ?? json.Count, json.Count);
			var step = Math.Max(_step ?? 1, 1);

			var index = start;
			while (index < end)
			{
				yield return json[index];
				index += step;
			}
		}
		public override string ToString()
		{
			return _step.HasValue
				       ? string.Format("{0}:{1}:{2}",
				                       _start.HasValue ? _start.ToString() : string.Empty,
				                       _end.HasValue ? _end.ToString() : string.Empty,
				                       _step)
				       : string.Format("{0}:{1}",
				                       _start.HasValue ? _start.ToString() : string.Empty,
				                       _end.HasValue ? _end.ToString() : string.Empty);

		}

		private static int ResolveIndex(int index, int count)
		{
			return index < 0 ? count + index : index;
		}
	}
}
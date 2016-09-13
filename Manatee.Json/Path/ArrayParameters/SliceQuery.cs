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
 
	File Name:		SliceQuery.cs
	Namespace:		Manatee.Json.Path.ArrayParameters
	Class Name:		SliceQuery
	Purpose:		Provides array-slice-syntax queries for arrays.

***************************************************************************************/
using Manatee.Json.Internal;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.ArrayParameters
{
	internal class SliceQuery : IJsonPathArrayQuery
	{
		private readonly IEnumerable<Slice> _slices;

		public SliceQuery(params Slice[] slices)
			: this((IEnumerable<Slice>) slices) {}
		public SliceQuery(IEnumerable<Slice> slices)
		{
			_slices = slices.ToList();
		}
		public IEnumerable<JsonValue> Find(JsonArray json, JsonValue root)
		{
			return _slices.SelectMany(s => s.Find(json, root)).Distinct();
		}
		public override string ToString()
		{
			return _slices.Join(",");
		}
	}
}
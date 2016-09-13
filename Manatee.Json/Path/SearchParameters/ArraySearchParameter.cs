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
 
	File Name:		ArraySearchParameter.cs
	Namespace:		Manatee.Json.Path.SearchParameters
	Class Name:		ArraySearchParameter
	Purpose:		Indicates that a search should look for an array parameter.

***************************************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.SearchParameters
{
	internal class ArraySearchParameter : IJsonPathSearchParameter, IEquatable<ArraySearchParameter>
	{
		private readonly IJsonPathArrayQuery _query;

		public ArraySearchParameter(IJsonPathArrayQuery query)
		{
			_query = query;
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
				{
					switch (v.Type)
					{
						case JsonValueType.Object:
							return v.Object.Values.SelectMany(jv => Find(new[] {jv}, root)).ToList();
						case JsonValueType.Array:
							var matches = _query.Find(v.Array, root).ToList();
							var search = v.Array.SelectMany(jv => Find(new[] {jv}, root));
							matches.AddRange(search);
							return matches;
						default:
							return Enumerable.Empty<JsonValue>();
					}
				}));
		}
		public override string ToString()
		{
			return $"[{_query}]";
		}
		public bool Equals(ArraySearchParameter other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(_query, other._query);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ArraySearchParameter);
		}
		public override int GetHashCode()
		{
			return _query?.GetHashCode() ?? 0;
		}
	}
}
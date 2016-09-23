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
 
	File Name:		NameSearchParameter.cs
	Namespace:		Manatee.Json.Path.SearchParameters
	Class Name:		NameSearchParameter
	Purpose:		Indicates that a search should look for a named parameter.

***************************************************************************************/
using Manatee.Json.Internal;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.SearchParameters
{
	internal class NameSearchParameter : IJsonPathSearchParameter, IEquatable<NameSearchParameter>
	{
		private readonly string _name;

		public NameSearchParameter(string name)
		{
			_name = name;
		}

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
			{
				switch (v.Type)
				{
					case JsonValueType.Object:
						var match = v.Object.ContainsKey(_name) ? v.Object[_name] : null;
						var search = v.Object.Values.SelectMany(jv => Find(new[] {jv}, root)).ToList();
						if (match != null)
							search.Insert(0, match);
						return search;
					case JsonValueType.Array:
						return new JsonArray(v.Array.SelectMany(jv => Find(new[] {jv}, root)));
					default:
						return Enumerable.Empty<JsonValue>();
				}
			}));
		}
		public override string ToString()
		{
			return _name.Any(c => !char.IsLetterOrDigit(c)) || _name.IsNullOrWhiteSpace()
					   ? $"'{_name}'"
					   : _name;
		}
		public bool Equals(NameSearchParameter other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return string.Equals(_name, other._name);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as NameSearchParameter);
		}
		public override int GetHashCode()
		{
			return _name?.GetHashCode() ?? 0;
		}
	}
}
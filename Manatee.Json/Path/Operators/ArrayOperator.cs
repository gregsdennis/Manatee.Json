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
 
	File Name:		ArrayOperator.cs
	Namespace:		Manatee.Json.Path.Operators
	Class Name:		ArrayOperator
	Purpose:		Indicates that the current value should be an array.

***************************************************************************************/
using System;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Operators
{
	internal class ArrayOperator : IJsonPathOperator, IEquatable<ArrayOperator>
	{
		public IJsonPathArrayQuery Query { get; }

		public ArrayOperator(IJsonPathArrayQuery query)
		{
			Query = query;
		}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v => v.Type == JsonValueType.Array
				                                          ? Query.Find(v.Array, root)
				                                          : v.Type == JsonValueType.Object
					                                          ? Query.Find(v.Object.Values.ToJson(), root)
					                                          : Enumerable.Empty<JsonValue>())
			                         .NotNull());
		}

		public override string ToString()
		{
			return $"[{Query}]";
		}
		public bool Equals(ArrayOperator other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Query, other.Query);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ArrayOperator);
		}
		public override int GetHashCode()
		{
			return Query?.GetHashCode() ?? 0;
		}
	}
}
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
			                         .WhereNotNull());
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
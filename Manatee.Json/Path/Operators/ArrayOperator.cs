using System;
using System.Collections.Generic;
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
			var results = new List<JsonValue>();

			foreach (var value in json)
			{
				_Evaluate(value, root, results);
			}

			return new JsonArray(results);
		}
		private void _Evaluate(JsonValue value, JsonValue root, List<JsonValue> results)
		{
			switch (value.Type)
			{
				case JsonValueType.Array:
					results.AddRange(Query.Find(value.Array, root));
					break;

				case JsonValueType.Object:
					results.AddRange(Query.Find(new JsonArray(value.Object.Values), root));
					break;
			}
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
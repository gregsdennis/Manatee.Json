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
			var results = new JsonArray();

			foreach (var value in json)
			{
				_Find(value, root, results);
			}

			return results;
		}
		private void _Find(JsonValue value, JsonValue root, JsonArray results)
		{
			switch (value.Type)
			{
				case JsonValueType.Object:
					foreach (var subValue in value.Object.Values)
					{
						_Find(subValue, root, results);
					}
					break;
				case JsonValueType.Array:
					results.AddRange(_query.Find(value.Array, root));
					foreach (var subValue in value.Array)
					{
						_Find(subValue, root, results);
					}
					break;
				default:
					break;
			}
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
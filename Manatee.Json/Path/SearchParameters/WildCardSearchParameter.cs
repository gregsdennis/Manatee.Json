using System;
using System.Collections.Generic;

namespace Manatee.Json.Path.SearchParameters
{
	internal class WildCardSearchParameter : IJsonPathSearchParameter, IEquatable<WildCardSearchParameter>
	{
		public static WildCardSearchParameter Instance { get; } = new WildCardSearchParameter();

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			var results = new JsonArray();
			foreach (var value in json)
			{
				_Find(value, results);
			}
			return results;
		}

		public override string? ToString()
		{
			return "*";
		}

		public bool Equals(WildCardSearchParameter? other)
		{
			return !ReferenceEquals(null, other);
		}

		public override bool Equals(object? obj)
		{
			return Equals(obj as WildCardSearchParameter);
		}

		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}

		private void _Find(JsonValue value, JsonArray results)
		{
			results.Add(value);
			switch (value.Type)
			{
				case JsonValueType.Object:
					foreach (var subValue in value.Object.Values)
					{
						_Find(subValue, results);
					}
					break;
				case JsonValueType.Array:
					foreach (var subValue in value.Array)
					{
						_Find(subValue, results);
					}
					break;
			}
		}
	}
}
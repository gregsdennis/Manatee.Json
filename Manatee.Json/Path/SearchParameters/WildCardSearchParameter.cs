using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.SearchParameters
{
	internal class WildCardSearchParameter : IJsonPathSearchParameter, IEquatable<WildCardSearchParameter>
	{
		public static WildCardSearchParameter Instance { get; } = new WildCardSearchParameter();

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
				{
					var contents = new List<JsonValue> {v};
					switch (v.Type)
					{
						case JsonValueType.Object:
							contents.AddRange(v.Object.Values.SelectMany(jv => Find(new JsonArray {jv}, root)));
							break;
						case JsonValueType.Array:
							contents.AddRange(v.Array.SelectMany(jv => Find(new JsonArray {jv}, root)));
							break;
					}
					return contents;
				}));
		}
		public override string ToString()
		{
			return "*";
		}
		public bool Equals(WildCardSearchParameter other)
		{
			return !ReferenceEquals(null, other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as WildCardSearchParameter);
		}
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
	}
}
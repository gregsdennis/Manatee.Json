using System;
using System.Collections.Generic;
using System.Linq;

namespace Manatee.Json.Path.SearchParameters
{
	internal class LengthSearchParameter : IJsonPathSearchParameter, IEquatable<LengthSearchParameter>
	{
		public static LengthSearchParameter Instance { get; } = new LengthSearchParameter();

		public IEnumerable<JsonValue> Find(IEnumerable<JsonValue> json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
				{
					var contents = new List<JsonValue> {v.Array.Count};
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
			return "length";
		}
		public bool Equals(LengthSearchParameter other)
		{
			return !ReferenceEquals(null, other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as LengthSearchParameter);
		}
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
	}
}
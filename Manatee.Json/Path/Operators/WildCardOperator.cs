using System;
using System.Linq;
using Manatee.Json.Internal;

namespace Manatee.Json.Path.Operators
{
	internal class WildCardOperator : IJsonPathOperator, IEquatable<WildCardOperator>
	{
		public static WildCardOperator Instance { get; }

		static WildCardOperator()
		{
			Instance = new WildCardOperator();
		}
		private WildCardOperator() {}

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return new JsonArray(json.SelectMany(v =>
				{
					switch (v.Type)
					{
						case JsonValueType.Object:
							return v.Object.Values;
						case JsonValueType.Array:
							return v.Array;
						default:
							return Enumerable.Empty<JsonValue>();
					}
				}).NotNull());
		}
		public override string ToString()
		{
			return ".*";
		}
		public bool Equals(WildCardOperator other)
		{
			return !ReferenceEquals(null, other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as WildCardOperator);
		}
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
	}
}
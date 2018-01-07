using System;

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
			var results = new JsonArray();
			foreach (var value in json)
			{
				_Evaluate(value, results);
			}
			return results;
		}

		private void _Evaluate(JsonValue value, JsonArray results)
		{
			switch(value.Type)
			{
				case JsonValueType.Object:
					results.AddRange(value.Object.Values);
					break;
				case JsonValueType.Array:
					results.AddRange(value.Array);
					break;
			}
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
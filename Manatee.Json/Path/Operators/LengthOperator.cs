using System;
using System.Linq;

namespace Manatee.Json.Path.Operators
{
	internal class LengthOperator : IJsonPathOperator, IEquatable<LengthOperator>
	{
		public static LengthOperator Instance { get; } = new LengthOperator();

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			return json.Where(v => v.Type == JsonValueType.Array)
			           .Select(v => (JsonValue) v.Array.Count)
			           .ToJson();
		}
		public override string ToString()
		{
			return ".length";
		}
		public bool Equals(LengthOperator other)
		{
			return !ReferenceEquals(null, other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as LengthOperator);
		}
		public override int GetHashCode()
		{
			// ReSharper disable once BaseObjectGetHashCodeCallInGetHashCode
			return base.GetHashCode();
		}
	}
}
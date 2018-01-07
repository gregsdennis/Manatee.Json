using System;
using System.Linq;

namespace Manatee.Json.Path.Operators
{
	internal class LengthOperator : IJsonPathOperator, IEquatable<LengthOperator>
	{
		public static LengthOperator Instance { get; } = new LengthOperator();

		public JsonArray Evaluate(JsonArray json, JsonValue root)
		{
			var results = new JsonArray();
			foreach (var value in json)
			{
				if (value.Type == JsonValueType.Array)
				{
					results.Add(value.Array.Count);
				}
			}
			return results;
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
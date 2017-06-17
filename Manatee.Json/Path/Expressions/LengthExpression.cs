using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class LengthExpression<T> : PathExpression<T>, IEquatable<LengthExpression<T>>
	{
		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			JsonArray array;
			if (Path == null || !Path.Operators.Any())
			{
				if (IsLocal)
					array = json as JsonArray;
				else
					array = root.Type == JsonValueType.Array ? root.Array : null;
				if (array == null)
				{
					var value = json as JsonValue;
					if (value?.Type == JsonValueType.Array)
						array = value.Array;
				}
				if (array == null) return null;
			}
			else
			{
				var value = IsLocal
								? json is JsonArray ? json as JsonArray : json as JsonValue
								: root;
				var results = Path.Evaluate(value);
				if (results.Count > 1)
					throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
				var result = results.FirstOrDefault();
				array = result != null && result.Type == JsonValueType.Array
					        ? result.Array
					        : null;
			}
			return array == null ? null : (object)(double) array.Count;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return (IsLocal ? "@" : "$") + $"{path}.length";
		}
		public bool Equals(LengthExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as LengthExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked
			{
				int hashCode = base.GetHashCode();
				hashCode = (hashCode * 397) ^ GetType().GetHashCode();
				return hashCode;
			}
		}
	}
}
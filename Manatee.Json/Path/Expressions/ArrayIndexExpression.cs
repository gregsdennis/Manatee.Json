using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class ArrayIndexExpression<T> : PathExpression<T>
	{
		public override int Priority { get { return 5; } }
		public int Index { get; set; }

		public override object Evaluate(T json)
		{
			var value = json as JsonValue;
			if (value == null)
				throw new NotSupportedException("ArrayIndex requires a JsonValue to evaluate.");
			var results = Path.Evaluate(json as JsonValue);
			if (results.Count > 1)
				throw new InvalidOperationException(string.Format("Path '{0}' returned more than one result on value '{1}'", Path, json));
			var result = results.FirstOrDefault();
			return result != null && result.Type == JsonValueType.Array && Index >= 0 && Index < result.Array.Count
					   ? result.Array[Index]
				       : null;
		}
		public override string ToString()
		{
			var path = Path == null ? string.Empty : Path.GetRawString();
			return string.Format("@{0}[{1}]", path, Index);
		}
	}
}
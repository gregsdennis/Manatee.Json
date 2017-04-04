using System;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class PathExpression<T> : ExpressionTreeNode<T>, IEquatable<PathExpression<T>>
	{
		public JsonPath Path { get; set; }
		public bool IsLocal { get; set; }

		protected override int BasePriority => 6;

		public override object Evaluate(T json, JsonValue root)
		{
			var value = IsLocal ? json as JsonValue : root;
			if (value == null)
				throw new InvalidOperationException($"Path must evaluate to a JsonValue. Returned value is {json.GetType().Name}.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			return results.FirstOrDefault();
		}
		public override string ToString()
		{
			return (IsLocal ? "@" : "$") + Path.GetRawString();
		}
		public bool Equals(PathExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Path, other.Path) && IsLocal == other.IsLocal;
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as PathExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Path?.GetHashCode() ?? 0)*397) ^ IsLocal.GetHashCode(); }
		}
	}
}
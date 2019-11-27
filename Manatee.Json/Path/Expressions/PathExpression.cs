using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Manatee.Json.Path.Expressions
{
	internal class PathExpression<T> : ExpressionTreeNode<T>, IEquatable<PathExpression<T>>
	{
		public JsonPath Path { get; }
		public bool IsLocal { get; }

		public PathExpression(JsonPath path, bool isLocal)
		{
			Path = path;
			IsLocal = isLocal;
		}

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var value = IsLocal ? json as JsonValue : root;
			if (value == null)
				throw new NotSupportedException("Path requires a JsonValue to evaluate.");
			var results = Path.Evaluate(value);
			if (results.Count > 1)
				throw new InvalidOperationException($"Path '{Path}' returned more than one result on value '{value}'");
			return results.FirstOrDefault();
		}
		public override string? ToString()
		{
			return (IsLocal ? "@" : "$") + Path.GetRawString();
		}
		public bool Equals(PathExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Path, other.Path) && IsLocal == other.IsLocal;
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as PathExpression<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Path?.GetHashCode() ?? 0)*397) ^ IsLocal.GetHashCode(); }
		}
	}
}
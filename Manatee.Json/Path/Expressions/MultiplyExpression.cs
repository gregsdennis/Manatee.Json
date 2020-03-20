using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class MultiplyExpression<T> : ExpressionTreeBranch<T>, IEquatable<MultiplyExpression<T>>
	{
		public MultiplyExpression(ExpressionTreeNode<T> left, ExpressionTreeNode<T> right)
			: base(left, right) { }

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (left is double dleft && right is double dright) return dleft * dright;
			return null;
		}
		public override string? ToString()
		{
			var left = Left is ExpressionTreeBranch<T> ? $"({Left})" : Left.ToString();
			var right = Right is ExpressionTreeBranch<T> ? $"({Right})" : Right.ToString();
			return $"{left}*{right}";
		}
		public bool Equals(MultiplyExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as MultiplyExpression<T>);
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
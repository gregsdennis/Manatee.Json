using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class OrExpression<T> : ExpressionTreeBranch<T>, IEquatable<OrExpression<T>>
	{
		public OrExpression(ExpressionTreeNode<T> left, ExpressionTreeNode<T> right)
			: base(left, right) { }

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var left = (bool?)Left.Evaluate(json, root);
			var right = (bool?)Right.Evaluate(json, root);
			return left.HasValue && right.HasValue &&
			       (left.Value || right.Value);
		}
		public override string? ToString()
		{
			var left = Left is ExpressionTreeBranch<T> ? $"({Left})" : Left.ToString();
			var right = Right is ExpressionTreeBranch<T> ? $"({Right})" : Right.ToString();
			return $"{left} || {right}";
		}
		public bool Equals(OrExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as OrExpression<T>);
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
using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class AddExpression<T> : ExpressionTreeBranch<T>, IEquatable<AddExpression<T>>
	{
		public AddExpression(ExpressionTreeNode<T> left, ExpressionTreeNode<T> right)
			: base(left, right) { }

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (left is double dleft && right is double dright) return dleft + dright;
			if (left is string sleft && right is string sright) return sleft + sright;
			return null;
		}
		public override string? ToString()
		{
			var right = Right is ExpressionTreeBranch<T> ? $"({Right})" : Right.ToString();
			return $"{Left}+{right}";
		}
		public bool Equals(AddExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object? obj)
		{
			if (ReferenceEquals(null, obj)) return false;
			if (ReferenceEquals(this, obj)) return true;
			if (obj.GetType() != this.GetType()) return false;
			return Equals((AddExpression<T>) obj);
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
using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class ExponentExpression<T> : ExpressionTreeBranch<T>, IEquatable<ExponentExpression<T>>
	{
		public ExponentExpression(ExpressionTreeNode<T> left, ExpressionTreeNode<T> right)
			: base(left, right) { }

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var left = Convert.ToDouble(Left.Evaluate(json, root));
			var right = Convert.ToDouble(Right.Evaluate(json, root));
			return Math.Pow(left, right);
		}
		public override string? ToString()
		{
			var left = Left is ExpressionTreeBranch<T> ? $"({Left})" : Left.ToString();
			var right = Right is ExpressionTreeBranch<T> ? $"({Right})" : Right.ToString();
			return $"{left}^{right}";
		}
		public bool Equals(ExponentExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as ExponentExpression<T>);
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
using System;

namespace Manatee.Json.Path.Expressions
{
	internal class IsGreaterThanExpression<T> : ExpressionTreeBranch<T>, IEquatable<IsGreaterThanExpression<T>>
	{
		protected override int BasePriority => 1;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			return ValueComparer.GreaterThan(left, right);
		}
		public override string ToString()
		{
			return $"{Left} > {Right}";
		}
		public bool Equals(IsGreaterThanExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as IsGreaterThanExpression<T>);
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
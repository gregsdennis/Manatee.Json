using System;

namespace Manatee.Json.Path.Expressions
{
	internal class SubtractExpression<T> : ExpressionTreeBranch<T>, IEquatable<SubtractExpression<T>>
	{
		protected override int BasePriority => 2;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (!(left is double) || !(right is double)) return null;
			return (double)left - (double)right;
		}
		public override string ToString()
		{
			if (Right?.Priority == Priority)
				return $"{Left}-({Right})";
			return $"{Left}-{Right}";
		}
		public bool Equals(SubtractExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as SubtractExpression<T>);
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
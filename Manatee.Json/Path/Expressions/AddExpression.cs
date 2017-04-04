using System;

namespace Manatee.Json.Path.Expressions
{
	internal class AddExpression<T> : ExpressionTreeBranch<T>, IEquatable<AddExpression<T>>
	{
		protected override int BasePriority => 2;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (!(left is double) || !(right is double))
			{
				var sleft = left as string;
				var sright = right as string;
				if (sleft == null || sright == null) return null;
				return sleft + sright;
			}
			return (double) left + (double) right;
		}
		public override string ToString()
		{
			return $"{Left}+{Right}";
		}
		public bool Equals(AddExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
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
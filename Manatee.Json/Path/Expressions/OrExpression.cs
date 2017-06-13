using System;

namespace Manatee.Json.Path.Expressions
{
	internal class OrExpression<T> : ExpressionTreeBranch<T>, IEquatable<OrExpression<T>>
	{
		protected override int BasePriority => 0;

		public override object Evaluate(T json, JsonValue root)
		{
			return (bool)Left.Evaluate(json, root) || (bool)Right.Evaluate(json, root);
		}
		public override string ToString()
		{
			return $"{Left} || {Right}";
		}
		public bool Equals(OrExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
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
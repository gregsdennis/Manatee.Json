using System;

namespace Manatee.Json.Path.Expressions
{
	internal class ModuloExpression<T> : ExpressionTreeBranch<T>, IEquatable<ModuloExpression<T>>
	{
		protected override int BasePriority => 2;

		public override object Evaluate(T json, JsonValue root)
		{
			var left = Left.Evaluate(json, root);
			var right = Right.Evaluate(json, root);
			if (!(left is double) || !(right is double)) return null;
			return (double) left%(double) right;
		}
		public override string ToString()
		{
			var left = Left.Priority <= Priority ? $"({Left})" : Left.ToString();
			var right = Right.Priority <= Priority ? $"({Right})" : Right.ToString();
			return $"{left}%{right}";
		}
		public bool Equals(ModuloExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return base.Equals(other);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ModuloExpression<T>);
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
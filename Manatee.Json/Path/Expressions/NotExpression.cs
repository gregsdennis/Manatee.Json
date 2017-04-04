using System;

namespace Manatee.Json.Path.Expressions
{
	internal class NotExpression<T> : ExpressionTreeNode<T>, IEquatable<NotExpression<T>>
	{
		public ExpressionTreeNode<T> Root { get; set; }

		protected override int BasePriority => 5;

		public override object Evaluate(T json, JsonValue root)
		{
			var result = Root.Evaluate(json, root);
			return result != null && result.Equals(true);
		}
		public override string ToString()
		{
			return Root is AndExpression<T> || Root is OrExpression<T> ||
				   Root is IsEqualExpression<T> || Root is IsNotEqualExpression<T> ||
				   Root is IsLessThanExpression<T> || Root is IsLessThanEqualExpression<T> ||
				   Root is IsGreaterThanExpression<T> || Root is IsGreaterThanEqualExpression<T>
					   ? $"!({Root})"
				       : $"!{Root}";
		}
		public bool Equals(NotExpression<T> other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Root, other.Root);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as NotExpression<T>);
		}
		public override int GetHashCode()
		{
			return Root?.GetHashCode() ?? 0;
		}
	}
}
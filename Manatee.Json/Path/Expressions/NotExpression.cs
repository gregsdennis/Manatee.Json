using System;
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal class NotExpression<T> : ExpressionTreeNode<T>, IEquatable<NotExpression<T>>
	{
		public ExpressionTreeNode<T> Root { get; }

		public NotExpression(ExpressionTreeNode<T> root)
		{
			Root = root;
		}

		public override object? Evaluate([MaybeNull] T json, JsonValue? root)
		{
			var result = Root.Evaluate(json, root);
			return result != null && result.Equals(true);
		}
		public override string? ToString()
		{
			return Root is ExpressionTreeBranch<T>
					   ? $"!({Root})"
				       : $"!{Root}";
		}
		public bool Equals(NotExpression<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Root, other.Root);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as NotExpression<T>);
		}
		public override int GetHashCode()
		{
			return Root.GetHashCode();
		}
	}
}
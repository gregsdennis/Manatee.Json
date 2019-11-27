namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeBranch<T> : ExpressionTreeNode<T>
	{
		public ExpressionTreeNode<T> Left { get; }
		public ExpressionTreeNode<T> Right { get; }

		public ExpressionTreeBranch(ExpressionTreeNode<T> left, ExpressionTreeNode<T> right)
		{
			Left = left;
			Right = right;
		}

		protected bool Equals(ExpressionTreeBranch<T>? other)
		{
			if (ReferenceEquals(null, other)) return false;
			if (ReferenceEquals(this, other)) return true;
			return Equals(Left, other.Left) && Equals(Right, other.Right);
		}
		public override bool Equals(object? obj)
		{
			return Equals(obj as ExpressionTreeBranch<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return (Left.GetHashCode()*397) ^ Right.GetHashCode(); }
		}
	}
}
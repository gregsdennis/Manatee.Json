namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeBranch<T> : ExpressionTreeNode<T>
	{
		public ExpressionTreeNode<T> Left { get; set; }
		public ExpressionTreeNode<T> Right { get; set; }

		protected bool Equals(ExpressionTreeBranch<T> other)
		{
			return Equals(Left, other.Left) && Equals(Right, other.Right);
		}
		public override bool Equals(object obj)
		{
			return Equals(obj as ExpressionTreeBranch<T>);
		}
		public override int GetHashCode()
		{
			unchecked { return ((Left?.GetHashCode() ?? 0)*397) ^ (Right?.GetHashCode() ?? 0); }
		}
	}
}
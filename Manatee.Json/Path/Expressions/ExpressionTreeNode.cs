namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeNode<T>
	{
		private int _priorityBump;

		public int Priority => BasePriority + _priorityBump;

		protected abstract int BasePriority { get; }

		public abstract object Evaluate(T json, JsonValue root);

		public void BumpPriority()
		{
			_priorityBump += 10;
		}
	}
}
namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeNode<T>
	{
		public abstract object Evaluate(T json, JsonValue root);
	}
}
using System.Diagnostics.CodeAnalysis;

namespace Manatee.Json.Path.Expressions
{
	internal abstract class ExpressionTreeNode<T>
	{
		public abstract object? Evaluate([MaybeNull] T json, JsonValue? root);
	}
}
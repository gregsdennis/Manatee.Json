using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class NullValueExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			return new ValueExpression<T> { Value = JsonValue.Null };
		}
	}
}
using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class IsGreaterThanExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var equal = body as BinaryExpression;
			if (equal == null)
				throw new InvalidOperationException();
			return new IsGreaterThanExpression<T>(ExpressionTranslator.TranslateNode<T>(equal.Left),
			                                      ExpressionTranslator.TranslateNode<T>(equal.Right));
		}
	}
}
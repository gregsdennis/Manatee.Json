using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class IsNotEqualExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var equal = body as BinaryExpression;
			if (equal == null)
				throw new InvalidOperationException();
			return new IsNotEqualExpression<T>(ExpressionTranslator.TranslateNode<T>(equal.Left),
			                                   ExpressionTranslator.TranslateNode<T>(equal.Right));
		}
	}
}
using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class IsLessThanEqualExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var equal = body as BinaryExpression;
			if (equal == null)
				throw new InvalidOperationException();
			return new IsLessThanEqualExpression<T>(ExpressionTranslator.TranslateNode<T>(equal.Left),
			                                        ExpressionTranslator.TranslateNode<T>(equal.Right));
		}
	}
}
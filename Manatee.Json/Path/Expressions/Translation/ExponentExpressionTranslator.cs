using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class ExponentExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var exp = body as BinaryExpression;
			if (exp == null)
				throw new InvalidOperationException();
			return new ExponentExpression<T>(ExpressionTranslator.TranslateNode<T>(exp.Left),
			                                 ExpressionTranslator.TranslateNode<T>(exp.Right));
		}
	}
}
using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class NegateExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var unary = body as UnaryExpression;
			if (unary == null)
				throw new InvalidOperationException();
			return new NegateExpression<T>(ExpressionTranslator.TranslateNode<T>(unary.Operand));
		}
	}
}
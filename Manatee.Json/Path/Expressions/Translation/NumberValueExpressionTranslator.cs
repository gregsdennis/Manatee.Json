using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class NumberValueExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var constant = body as ConstantExpression;
			if (constant == null)
				throw new InvalidOperationException();
			return new ValueExpression<T> { Value = Convert.ToDouble(constant.Value) };
		}
	}
}
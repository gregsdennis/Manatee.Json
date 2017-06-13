using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class OrExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var and = body as BinaryExpression;
			if (and == null)
				throw new InvalidOperationException();
			return new OrExpression<T>
				{
					Left = ExpressionTranslator.TranslateNode<T>(and.Left),
					Right = ExpressionTranslator.TranslateNode<T>(and.Right)
				};
		}
	}
}
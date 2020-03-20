using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class DivideExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var divide = body as BinaryExpression;
			if (divide == null)
				throw new InvalidOperationException();
			return new DivideExpression<T>(ExpressionTranslator.TranslateNode<T>(divide.Left),
			                               ExpressionTranslator.TranslateNode<T>(divide.Right));
		}
	}
}
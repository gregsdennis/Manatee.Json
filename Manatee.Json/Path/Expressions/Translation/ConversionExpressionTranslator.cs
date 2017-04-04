using System;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class ConversionExpressionTranslator : IExpressionTranslator
	{
		public ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var unary = body as UnaryExpression;
			if (unary == null)
				throw new InvalidOperationException();
			return new ConversionExpression<T>
				{
					Root = ExpressionTranslator.TranslateNode<T>(unary.Operand),
					TargetType = unary.Type
				};
		}
	}
}
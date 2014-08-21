using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class ArrayIndexExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(int))
				throw new NotSupportedException("Only literal string arguments are supported in Name().");
			return new ArrayIndexExpression<T>
				{
					Path = BuildPath(method),
					Index = (int) parameter.Value
				};
		}
	}
}
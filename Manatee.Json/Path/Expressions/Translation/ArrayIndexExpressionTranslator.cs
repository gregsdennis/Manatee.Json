using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class ArrayIndexExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(int))
			{
				return new ArrayIndexExpression<T>
					{
						Path = BuildPath(method, out isLocal),
						IsLocal = isLocal,
						IndexExpression = ExpressionTranslator.TranslateNode<T>(method.Arguments.Last())
					};
			}
			return new ArrayIndexExpression<T>
				{
					Path = BuildPath(method, out isLocal),
					IsLocal = isLocal,
					Index = (int) parameter.Value,
				};
		}
	}
}
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class NameExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof (string))
			{
				return new NameExpression<T>
				{
					Path = BuildPath(method, out isLocal),
					IsLocal = isLocal,
					NameExp = ExpressionTranslator.TranslateNode<T>(method.Arguments.Last())
				};
			}
			return new NameExpression<T>
				{
					Path = BuildPath(method, out isLocal),
					IsLocal = isLocal,
					Name = (string)parameter.Value
				};
		}
	}
}
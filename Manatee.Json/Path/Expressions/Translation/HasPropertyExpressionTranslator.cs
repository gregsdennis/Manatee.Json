using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class HasPropertyExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(string))
				throw new NotSupportedException("Only constant string arguments are supported in HasProperty()");
			return new HasPropertyExpression<T>
				{
					Path = BuildPath(method, out bool isLocal),
					IsLocal = isLocal,
					Name = parameter.Value.ToString()
				};
		}
	}
}
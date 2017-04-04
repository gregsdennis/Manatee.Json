using System;
using System.Linq;
using System.Linq.Expressions;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal class IndexOfExpressionTranslator : PathExpressionTranslator
	{
		public override ExpressionTreeNode<T> Translate<T>(Expression body)
		{
			bool isLocal;
			var method = body as MethodCallExpression;
			if (method == null)
				throw new InvalidOperationException();
			var parameter = method.Arguments.Last() as ConstantExpression;
			if (parameter == null || parameter.Type != typeof(JsonValue))
			{
				return new IndexOfExpression<T>
					{
						Path = BuildPath(method, out isLocal),
						IsLocal = isLocal,
						ParameterExpression = ExpressionTranslator.TranslateNode<JsonArray>(method.Arguments.Last())
					};
			}
			return new IndexOfExpression<T>
				{
					Path = BuildPath(method, out isLocal),
					IsLocal = isLocal,
					Parameter = (JsonValue)parameter.Value,
				};
		}
	}
}
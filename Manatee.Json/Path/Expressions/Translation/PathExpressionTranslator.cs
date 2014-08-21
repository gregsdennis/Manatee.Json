using System;
using System.Linq;
using System.Linq.Expressions;
using Manatee.Json.Path.Operators;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal abstract class PathExpressionTranslator : IExpressionTranslator
	{
		public abstract ExpressionTreeNode<T> Translate<T>(Expression body);

		protected static JsonPath BuildPath(MethodCallExpression method)
		{
			var path = new JsonPath();
			var currentMethod = method.Arguments.First() as MethodCallExpression;
			while (currentMethod != null)
			{
				var parameter = currentMethod.Arguments.Last() as ConstantExpression;
				if (parameter != null && parameter.Type != typeof(string))
					throw new NotSupportedException("Only literal string arguments are supported");
				switch (currentMethod.Method.Name)
				{
					case "Name":
						path.Insert(0, new NameOperator((string) parameter.Value));
						break;
				}
				currentMethod = currentMethod.Arguments.First() as MethodCallExpression;
			}
			return path;
		}
	}
}
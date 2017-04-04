using System;
using System.Linq;
using System.Linq.Expressions;
using Manatee.Json.Path.ArrayParameters;
using Manatee.Json.Path.Operators;

namespace Manatee.Json.Path.Expressions.Translation
{
	internal abstract class PathExpressionTranslator : IExpressionTranslator
	{
		public abstract ExpressionTreeNode<T> Translate<T>(Expression body);

		protected static JsonPath BuildPath(MethodCallExpression method, out bool isLocal)
		{
			var path = new JsonPath();
			var currentMethod = method.Arguments.FirstOrDefault() as MethodCallExpression;
			isLocal = method.Method.Name == "Length" ? method.Arguments.Count != 0 : method.Arguments.Count != 1;
			while (currentMethod != null)
			{
				var parameter = currentMethod.Arguments.Last() as ConstantExpression;
				switch (currentMethod.Method.Name)
				{
					case "Name":
						if (parameter == null || parameter.Type != typeof (string))
							throw new NotSupportedException("Only literal string arguments are supported within JsonPath expressions.");
						path.Operators.Insert(0, new NameOperator((string) parameter.Value));
						break;
					case "ArrayIndex":
						if (parameter == null || parameter.Type != typeof (int))
							throw new NotSupportedException("Only literal string arguments are supported within JsonPath expressions.");
						path.Operators.Insert(0, new ArrayOperator(new SliceQuery(new Slice((int) parameter.Value))));
						break;
				}
				isLocal = currentMethod.Arguments.Count != 1;
				currentMethod = currentMethod.Arguments.FirstOrDefault() as MethodCallExpression;
			}
			return path;
		}
	}
}
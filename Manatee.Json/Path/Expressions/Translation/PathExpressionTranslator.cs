/***************************************************************************************

	Copyright 2016 Greg Dennis

	   Licensed under the Apache License, Version 2.0 (the "License");
	   you may not use this file except in compliance with the License.
	   You may obtain a copy of the License at

		 http://www.apache.org/licenses/LICENSE-2.0

	   Unless required by applicable law or agreed to in writing, software
	   distributed under the License is distributed on an "AS IS" BASIS,
	   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
	   See the License for the specific language governing permissions and
	   limitations under the License.
 
	File Name:		PathExpressionTranslator.cs
	Namespace:		Manatee.Json.Path.Expressions.Translation
	Class Name:		PathExpressionTranslator
	Purpose:		Base class for expression translators which build a path
					within an expression.

***************************************************************************************/
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
			var currentMethod = method.Arguments.First() as MethodCallExpression;
			isLocal = method.Method.Name== "Length" ? method.Arguments.Count != 0 : method.Arguments.Count != 1;
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
				currentMethod = currentMethod.Arguments.First() as MethodCallExpression;
			}
			return path;
		}
	}
}